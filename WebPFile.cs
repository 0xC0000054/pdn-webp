using System;
using System.Runtime.InteropServices;
using WebPFileType.Properties;

namespace WebPFileType
{
	static class WebPFile
	{
		internal enum VP8StatusCode : int
		{
			Ok = 0,
			OutOfMemory,
			InvalidParam,
			BitStreamError,
			UnsupportedFeature,
			Suspended,
			UserAbort,
			NotEnoughData,
		}

		internal enum MetaDataType : int
		{ 
			ColorProfile = 0,
			EXIF,
			XMP
		}

		internal enum WebPMuxError : int
		{
			Ok = 1,
			NotFound = 0,
			InvalidArgument = -1,
			BadData = -2,
			MemoryError = -3,
			NotEnoughData = -4
		}

		internal enum WebPEncodingError : int
		{
			ApiVersionMismatch = -1,
			Ok = 0,
			OutOfMemory = 1,           // memory error allocating objects
			BitStreamOutOfMemory = 2, // memory error while flushing bits
			NullParameter = 3,         // a pointer parameter is NULL
			InvalidConfiguration = 4,   // configuration is invalid
			BadDimension = 5,           // picture has invalid width/height
			PartitionZeroOverflow = 6,     // partition is bigger than 512k
			PartitionOverflow = 7,      // partition is bigger than 16M
			BadWrite = 8,               // error while flushing bytes
			FileTooBig = 9,            // file is bigger than 4G
			UserAbort = 10              // abort request by user
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void WebPReportProgress(int progress);

		[StructLayout(LayoutKind.Sequential)]
		internal struct EncodeParams
		{
			[MarshalAs(UnmanagedType.R4)]
			public float quality;
			[MarshalAs(UnmanagedType.I4)]
			public WebPPreset preset;			
			public int method;
			public int noiseShaping;
			[MarshalAs(UnmanagedType.I4)]
			public WebPFilterType filterType;
			public int filterStrength;
			public int sharpness;
			public int fileSize;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MetaDataParams
		{
			public byte[] iccProfile;
			public uint iccProfileSize;
			public byte[] exif;
			public uint exifSize;
			public byte[] xmp;
			public uint xmpSize;
		}
		
		private const int WebPMaxDimension = 16383;
		[System.Security.SuppressUnmanagedCodeSecurity]
		private unsafe static class WebP_32
		{
			[DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPGetDimensions")]
			[return: MarshalAs(UnmanagedType.I1)]
			public static unsafe extern bool WebPGetDimensions(byte* data, UIntPtr dataSize, out int width, out int height);

			[DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPFreeMemory")]
			public static extern void WebPFreeMemory(IntPtr mem);

			[DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPLoad")]
			public static unsafe extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, byte* outData, int outSize, int outStride);

			[DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPSave")]
			public static extern WebPEncodingError WebPSave(out IntPtr output, out uint outputSize, IntPtr iBitmap, int iWidth, int iHeight, int iStride, EncodeParams parameters, WebPReportProgress callback);

			[DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMetaDataSize")]
			public static unsafe extern void GetMetaDataSize(byte* iData, UIntPtr iDataSize, MetaDataType type, out uint metaDataSize);

			[DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExtractMetaData")]
			public static unsafe extern void ExtractMetaData(byte* iData, UIntPtr iDataSize, byte* metaDataBytes, uint metaDataSize, MetaDataType type);

			[DllImportAttribute("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetMetaData")]
			public static unsafe extern WebPMuxError SetMetaData(byte* image, UIntPtr imageSize, ref IntPtr outImage, ref UIntPtr outImageSize, MetaDataParams metaData);
		}

		[System.Security.SuppressUnmanagedCodeSecurity]
		private unsafe static class WebP_64
		{
			[DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPGetDimensions")]
			[return: MarshalAs(UnmanagedType.I1)]
			public static unsafe extern bool WebPGetDimensions(byte* data, UIntPtr dataSize, out int width, out int height);

			[DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPFreeMemory")]
			public static extern void WebPFreeMemory(IntPtr mem);

			[DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPLoad")]
			public static unsafe extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, byte* outData, int outSize, int outStride);

			[DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPSave")]
			public static unsafe extern WebPEncodingError WebPSave(out IntPtr output, out uint outputSize, IntPtr scan0, int iWidth, int iHeight, int iStride, EncodeParams parameters, WebPReportProgress callback);

			[DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMetaDataSize")]
			public static unsafe extern void GetMetaDataSize(byte* iData, UIntPtr iDataSize, MetaDataType type, out uint metaDataSize);

			[DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExtractMetaData")]
			public static unsafe extern void ExtractMetaData(byte* iData, UIntPtr iDataSize, byte* metaDataBytes, uint metaDataSize, MetaDataType type);

			[DllImportAttribute("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetMetaData")]
			public static unsafe extern WebPMuxError SetMetaData(byte* image, UIntPtr imageSize, ref IntPtr outImage, ref UIntPtr outImageSize, MetaDataParams metaData);
		}

		/// <summary>
		/// Frees the unmanaged memory allocates with <see cref="WebPAllocateMemory"/>.
		/// </summary>
		/// <param name="memory">The memory to free.</param>
		private static void WebPFreeMemory(IntPtr memory)
		{
			if (IntPtr.Size == 8)
			{
				WebP_64.WebPFreeMemory(memory);
			}
			else
			{
				WebP_32.WebPFreeMemory(memory);
			}
		}

		/// <summary>
		/// Gets the dimension of the WebP image.
		/// </summary>
		/// <param name="data">The input image data.</param>
		/// <param name="dataSize">The size of the input data.</param>
		/// <param name="width">The output width of the image.</param>
		/// <param name="height">The output height of the image.</param>
		/// <returns>true on success, otherwise false.</returns>
		internal static unsafe bool WebPGetDimensions(byte[] data, uint dataSize, out int width, out int height)
		{
			fixed (byte* ptr = data)
			{
				if (IntPtr.Size == 8)
				{
					return WebP_64.WebPGetDimensions(ptr, (UIntPtr)dataSize, out width, out height);
				}
				else
				{
					return WebP_32.WebPGetDimensions(ptr, (UIntPtr)dataSize, out width, out height);
				} 
			}
		}

		/// <summary>
		/// The WebP load function.
		/// </summary>
		/// <param name="data">The input image data</param>
		/// <param name="dataSize">Size of the data.</param>
		/// <param name="width">The width 0f the resulting image.</param>
		/// <param name="outputStride">The height of the resulting image.</param>
		/// <param name="outPtr">The output byte array.</param>
		/// <returns>VP8StatusCode.Ok on success.</returns>
		internal static unsafe VP8StatusCode WebPLoad(byte[] data, uint dataSize, byte* outPtr, int outputSize, int outputStride)
		{
			fixed (byte* ptr = data)
			{ 
				if (IntPtr.Size == 8)
				{
					return WebP_64.WebPLoad(ptr, (UIntPtr)dataSize, outPtr, outputSize, outputStride);
				}
				else
				{
				   return WebP_32.WebPLoad(ptr, (UIntPtr)dataSize, outPtr, outputSize, outputStride);
				}
			}
		}

		/// <summary>
		/// The WebP save function.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="scan0">The input bitmap.</param>
		/// <param name="width">Width of the input bitmap.</param>
		/// <param name="height">Height of the input bitmap.</param>
		/// <param name="stride">The stride of the input bitmap.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="callback">The callback.</param>
		internal static byte[] WebPSave(IntPtr scan0, int width, int height, long stride, EncodeParams parameters, WebPReportProgress callback)
		{
			if (width > WebPMaxDimension || height > WebPMaxDimension)
			{
				throw new FormatException(Resources.InvalidImageDimensions);
			}


			WebPEncodingError retVal = WebPEncodingError.Ok;
			IntPtr data = IntPtr.Zero;
			uint dataSize = 0U;
			byte[] output = null;

			try
			{
				
				if (IntPtr.Size == 8)
				{
					retVal = WebP_64.WebPSave(out data, out dataSize, scan0, width, height, (int)stride, parameters, callback);
				}
				else
				{
					retVal = WebP_32.WebPSave(out data, out dataSize, scan0, width, height, (int)stride, parameters, callback);
				}

				if (retVal == WebPEncodingError.Ok)
				{
#if DEBUG
					System.Diagnostics.Debug.Assert(data != IntPtr.Zero);
#endif
					output = new byte[dataSize];
					Marshal.Copy(data, output, 0, output.Length);
				}
				else
				{
					switch (retVal)
					{
						case WebPEncodingError.OutOfMemory:
						case WebPEncodingError.BitStreamOutOfMemory:
							throw new OutOfMemoryException(Resources.InsufficientMemoryOnSave);
						case WebPEncodingError.NullParameter:
						case WebPEncodingError.InvalidConfiguration:
						case WebPEncodingError.PartitionZeroOverflow:
						case WebPEncodingError.PartitionOverflow:
						case WebPEncodingError.BadWrite:
							throw new WebPException(Resources.EncoderGenericError);
						case WebPEncodingError.FileTooBig:
							throw new WebPException(Resources.EncoderFileTooBig);

						case WebPEncodingError.ApiVersionMismatch:
							throw new WebPException(Resources.ApiVersionMismatch);
					}
				}
			}
			finally
			{
				if (data != IntPtr.Zero)
				{
					WebPFreeMemory(data);
					data = IntPtr.Zero;
				}
			}

			return output;
		}

		internal static unsafe uint GetMetaDataSize(byte[] data, uint dataSize, MetaDataType type)
		{
			uint metaDataSize = 0U;

			fixed (byte* ptr = data)
			{
				if (IntPtr.Size == 8)
				{
					WebP_64.GetMetaDataSize(ptr, (UIntPtr)dataSize, type, out metaDataSize);
				}
				else
				{
					WebP_32.GetMetaDataSize(ptr, (UIntPtr)dataSize, type, out metaDataSize);
				}
			}

			return metaDataSize;
		}

		internal static unsafe void ExtractMetadata(byte[] data, uint dataSize, MetaDataType type, byte[] outData, uint outSize)
		{
			fixed (byte* ptr = data, outPtr = outData)
			{
				if (IntPtr.Size == 8)
				{
					WebP_64.ExtractMetaData(ptr, (UIntPtr)dataSize, outPtr, outSize, type);
				}
				else
				{
					WebP_32.ExtractMetaData(ptr, (UIntPtr)dataSize, outPtr, outSize, type);
				}
			}
		}

		internal static unsafe byte[] SetMetaData(byte[] data, uint dataSize, MetaDataParams metaData)
		{
			byte[] outImage = null;

			WebPMuxError error = WebPMuxError.Ok;

			fixed (byte* ptr = data)
			{
				IntPtr outPtr = IntPtr.Zero; 
				UIntPtr outSize = UIntPtr.Zero;

				try
				{
					if (IntPtr.Size == 8)
					{
						error = WebP_64.SetMetaData(ptr, (UIntPtr)dataSize, ref outPtr, ref outSize, metaData);
					}
					else
					{
						error = WebP_32.SetMetaData(ptr, (UIntPtr)dataSize, ref outPtr, ref outSize, metaData);
					}

					if (error == WebPMuxError.Ok)
					{
						int size = (int)outSize.ToUInt32();

						outImage = new byte[size];
						Marshal.Copy(outPtr, outImage, 0, outImage.Length);
					}
					else
					{
						switch (error)
						{
							case WebPMuxError.MemoryError:
								throw new OutOfMemoryException(Resources.InsufficientMemoryOnSave);
							case WebPMuxError.NotFound:
							case WebPMuxError.InvalidArgument:
							case WebPMuxError.BadData:
							case WebPMuxError.NotEnoughData:
								throw new WebPException(Resources.EncoderGenericError);
							default:
								break;
						}

					}
				}
				finally
				{
					if (outPtr != IntPtr.Zero)
					{
						WebPFreeMemory(outPtr);
						outPtr = IntPtr.Zero;
					}
				}

			}

			return outImage;
		}


	}
}
