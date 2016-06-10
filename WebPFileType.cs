using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PaintDotNet;
using PaintDotNet.IO;
using WebPFileType.Properties;

namespace WebPFileType
{
	[PaintDotNet.PluginSupportInfo(typeof(PluginSupportInfo))]
	public sealed class WebPFileType : FileType, IFileTypeFactory
	{
		private enum PropertyNames
		{
			FilterStrength,
			FilterType,
			Method,			
			Preset, 
			Quality,
			Sharpness,
			FileSize
		}
		private WebPFile.EncodeParams encParams;
		private const string WebPColorProfile = "WebPICC";
		private const string WebPEXIF = "WebPEXIF";
		private const string WebPXMP = "WebPXMP";

		public WebPFileType() : base("WebP", FileTypeFlags.SupportsLoading | FileTypeFlags.SupportsSaving | FileTypeFlags.SavesWithProgress, new string[] { ".webp" })
		{
			encParams = new WebPFile.EncodeParams();
		}

		public FileType[] GetFileTypeInstances()
		{
			return new FileType[] { new WebPFileType()};
		}

		private static string GetMetaDataBase64(byte[] data, WebPFile.MetaDataType type, uint metaDataSize)
		{
			byte[] bytes = new byte[metaDataSize];
			WebPFile.ExtractMetadata(data, type, bytes, metaDataSize);

			return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		}

		protected override Document OnLoad(System.IO.Stream input)
		{
			byte[] bytes = new byte[input.Length];

			input.ProperRead(bytes, 0, (int)input.Length);

			try
			{
				int width = 0;
				int height = 0;
				if (!WebPFile.WebPGetDimensions(bytes, out width, out height))
				{
					throw new WebPException(Resources.InvalidWebPImage);
				}

				Document doc = new Document(width, height);
				BitmapLayer layer = Layer.CreateBackgroundLayer(width, height);

				int stride = layer.Surface.Stride;
				int bitmapSize = stride * layer.Surface.Height;

				unsafe
				{
					WebPFile.VP8StatusCode status = WebPFile.WebPLoad(bytes, (byte*)layer.Surface.Scan0.VoidStar, bitmapSize, stride);
					if (status != WebPFile.VP8StatusCode.Ok)
					{
						switch (status)
						{
							case WebPFile.VP8StatusCode.OutOfMemory:
								throw new OutOfMemoryException();
							case WebPFile.VP8StatusCode.InvalidParam:
								throw new WebPException(Resources.InvalidParameter);
							case WebPFile.VP8StatusCode.BitStreamError:
							case WebPFile.VP8StatusCode.UnsupportedFeature:
							case WebPFile.VP8StatusCode.NotEnoughData:
								throw new WebPException(Resources.InvalidWebPImage);
							default:
								break;
						}
					}

					uint colorProfileSize = WebPFile.GetMetaDataSize(bytes, WebPFile.MetaDataType.ColorProfile);
					if (colorProfileSize > 0U)
					{
						string icc = GetMetaDataBase64(bytes, WebPFile.MetaDataType.ColorProfile, colorProfileSize);
						doc.Metadata.SetUserValue(WebPColorProfile, icc);
					}

					uint exifSize = WebPFile.GetMetaDataSize(bytes, WebPFile.MetaDataType.EXIF);
					if (exifSize > 0)
					{
						string exif = GetMetaDataBase64(bytes, WebPFile.MetaDataType.EXIF, exifSize);
						doc.Metadata.SetUserValue(WebPEXIF, exif);
					}

					uint xmpSize = WebPFile.GetMetaDataSize(bytes, WebPFile.MetaDataType.XMP);
					if (xmpSize > 0U)
					{
						string xmp = GetMetaDataBase64(bytes, WebPFile.MetaDataType.XMP, xmpSize);
						doc.Metadata.SetUserValue(WebPXMP, xmp);
					}
				}
					   
				doc.Layers.Add(layer);

				return doc;
			}
			catch (WebPException ex)
			{
				MessageBox.Show(ex.Message, "Error loading WebP Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}

		}

		public override SaveConfigWidget CreateSaveConfigWidget()
		{
			return new WebPSaveConfigWidget();
		}

		protected override SaveConfigToken OnCreateDefaultSaveConfigToken()
		{
			return new WebPSaveConfigToken(WebPPreset.Photo, 95, 4, 80, 30, 3, WebPFilterType.Strong, 0, true);
		}

		public override bool IsReflexive(SaveConfigToken token)
		{
			if (((WebPSaveConfigToken)token).Quality == 100)
			{
				return true;
			}

			return false;
		}

		private static void LoadProperties(Image dstImage, Document srcDoc)
		{
			Bitmap asBitmap = dstImage as Bitmap;

			if (asBitmap != null)
			{
				// Sometimes GDI+ does not honor the resolution tags that we
				// put in manually via the EXIF properties.
				float dpiX;
				float dpiY;

				switch (srcDoc.DpuUnit)
				{
					case MeasurementUnit.Centimeter:
						dpiX = (float)Document.DotsPerCmToDotsPerInch(srcDoc.DpuX);
						dpiY = (float)Document.DotsPerCmToDotsPerInch(srcDoc.DpuY);
						break;

					case MeasurementUnit.Inch:
						dpiX = (float)srcDoc.DpuX;
						dpiY = (float)srcDoc.DpuY;
						break;

					default:
					case MeasurementUnit.Pixel:
						dpiX = 1.0f;
						dpiY = 1.0f;
						break;
				}

				try
				{
					asBitmap.SetResolution(dpiX, dpiY);
				}
				catch (Exception)
				{
					// Ignore error
				}
			}

			Metadata metaData = srcDoc.Metadata;

			foreach (string key in metaData.GetKeys(Metadata.ExifSectionName))
			{
				string blob = metaData.GetValue(Metadata.ExifSectionName, key);
				System.Drawing.Imaging.PropertyItem pi = PaintDotNet.SystemLayer.PdnGraphics.DeserializePropertyItem(blob);

				try
				{
					dstImage.SetPropertyItem(pi);
				}
				catch (ArgumentException)
				{
					// Ignore error: the image does not support property items
				}
			}
		}

		private static IntPtr EncodeMetaData(Document doc, Surface scratchSurface, byte[] imageData, PinnedByteArrayAllocator output)
		{
			WebPFile.MetaDataParams metaData = new WebPFile.MetaDataParams();

			string colorProfile = doc.Metadata.GetUserValue(WebPColorProfile);
			if (!string.IsNullOrEmpty(colorProfile))
			{
				metaData.iccProfile = Convert.FromBase64String(colorProfile);
				metaData.iccProfileSize = (uint)metaData.iccProfile.Length;
			}

			string exif = doc.Metadata.GetUserValue(WebPEXIF);
			if (!string.IsNullOrEmpty(exif))
			{
				metaData.exif = Convert.FromBase64String(exif);
				metaData.exifSize = (uint)metaData.exif.Length;
			}
			else if (doc.Metadata.GetKeys(Metadata.ExifSectionName).Length > 0)
			{
				byte[] exifBytes = null;
				using (MemoryStream stream = new MemoryStream())
				{
					using (Bitmap bmp = scratchSurface.CreateAliasedBitmap())
					{
						LoadProperties(bmp, doc);
						bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
					}

					exifBytes = JPEGReader.ExtractEXIF(stream.GetBuffer());
				}

				if (exifBytes != null)
				{
					metaData.exif = exifBytes;
					metaData.exifSize = (uint)exifBytes.Length;
				}
				
			}

			string xmp = doc.Metadata.GetUserValue(WebPXMP);
			if (!string.IsNullOrEmpty(xmp))
			{
				metaData.xmp = Convert.FromBase64String(xmp);
				metaData.xmpSize = (uint)metaData.xmp.Length;
			}

			if (metaData.iccProfileSize > 0U || metaData.exifSize > 0U || metaData.xmpSize > 0U)
			{
				return WebPFile.SetMetaData(imageData, metaData, output);
			}

			return IntPtr.Zero;
		}

		protected override void OnSave(Document input, System.IO.Stream output, SaveConfigToken token, Surface scratchSurface, ProgressEventHandler callback)
		{
			WebPSaveConfigToken configToken = (WebPSaveConfigToken)token;

			WebPFile.WebPReportProgress encProgress = new WebPFile.WebPReportProgress(delegate(int percent)
			{
				callback(this, new ProgressEventArgs(percent));
			});

			encParams.quality = (float)configToken.Quality;
			encParams.preset = configToken.Preset;
			encParams.method = configToken.Method;
			encParams.noiseShaping = configToken.NoiseShaping;
			encParams.filterType = configToken.FilterType;
			encParams.filterStrength = configToken.FilterStrength;
			encParams.sharpness = configToken.Sharpness;
			encParams.fileSize = configToken.FileSize;
		
			using (RenderArgs ra = new RenderArgs(scratchSurface))
			{
				input.Render(ra, true);
			}

			using (PinnedByteArrayAllocator allocator = new PinnedByteArrayAllocator())
			{
				IntPtr pinnedArrayPtr = WebPFile.WebPSave(
					allocator,
					scratchSurface.Scan0.Pointer,
					scratchSurface.Width,
					scratchSurface.Height,
					scratchSurface.Stride,
					encParams, 
					encProgress);

				if (pinnedArrayPtr != IntPtr.Zero)
				{
					IntPtr imageDataPtr = pinnedArrayPtr;
					if (configToken.EncodeMetaData)
					{
						IntPtr metaDataPtr = EncodeMetaData(input, scratchSurface, allocator.GetManagedArray(pinnedArrayPtr), allocator);
						if (metaDataPtr != IntPtr.Zero)
						{
							imageDataPtr = metaDataPtr;
						}
					}
					byte[] data = allocator.GetManagedArray(imageDataPtr);

					output.Write(data, 0, data.Length);
				} 
			}
		}
	}
}
