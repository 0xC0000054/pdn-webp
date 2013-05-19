using System;
using System.Runtime.InteropServices;
using WebPFileType.Properties;

namespace WebPFileType
{
	class WebPFile
	{

		internal enum VP8StatusCode 
		{
			/// VP8_STATUS_OK -> 0
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

		internal enum WebPMuxError
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
			VP8_ENC_ERROR_BAD_DIMENSION = 5,           // picture has invalid width/height
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
			[MarshalAs(UnmanagedType.I4)]
			public WebPFilterType filterType;
			public int filterStrength;
			public int sharpness;	
			public int method;
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

			/// Return Type: int
			///iData: uint8_t*
			///iData_size: uint32_t->unsigned int
			///oWidth: int*
			///oHeight: int*
			[System.Runtime.InteropServices.DllImportAttribute("WebP_x86.dll", EntryPoint = "WebPGetDimensions")]
			[return: MarshalAs(UnmanagedType.I1)]
			public static unsafe extern bool WebPGetDimensions(byte* data, UIntPtr dataSize, out int width, out int height);

			/// Return Type: void
			///mem: void*
			[System.Runtime.InteropServices.DllImportAttribute("WebP_x86.dll", EntryPoint = "WebPFreeMemory")]
			public static extern void WebPFreeMemory(System.IntPtr mem);


			/// Return Type: uint8_t*
			///iBitmap: void*
			///iBitmapSize: int
			///iStride: int
			///iWebPFile: uint8_t*
			///iWebPFileSize: uint32_t->unsigned int
			[System.Runtime.InteropServices.DllImportAttribute("WebP_x86.dll", EntryPoint = "WebPLoad")]
			public static unsafe extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, ref byte* outData, int outSize, int outStride);

			/// Return Type: int
			///output: void**
			///outputSize: uint32_t->unsigned int
			///iBitmap: void*
			///iBitmapSize: size_t->unsigned int
			///iWidth: int
			///iHeight: int
			///iStride: int
			///iQuality: float
			///iMethod: int
			///iSharpness: int
			///iPreset: WebPPreset->Anonymous_0f2b80ba_5d6f_409d_9e5a_ef9696863a72
			[System.Runtime.InteropServices.DllImportAttribute("WebP_x86.dll", EntryPoint = "WebPSave")]
			public static extern WebPEncodingError WebPSave(ref IntPtr output, out uint outputSize, IntPtr iBitmap, int iWidth, int iHeight, int iStride, EncodeParams parameters, WebPReportProgress callback);

			[DllImport("WebP_x86.dll", EntryPoint = "GetMetaDataSize")]
			public static unsafe extern void GetMetaDataSize(byte* iData, UIntPtr iDataSize, MetaDataType type, out uint metaDataSize);

			[DllImport("WebP_x86.dll", EntryPoint = "ExtractMetaData")]
			public static unsafe extern void ExtractMetaData(byte* iData, UIntPtr iDataSize, byte* metaDataBytes, uint metaDataSize, MetaDataType type);


			[DllImportAttribute("WebP_x86.dll", EntryPoint = "SetMetaData")]
			public static unsafe extern WebPMuxError SetMetaData(byte* image, UIntPtr imageSize, ref IntPtr outImage, ref UIntPtr outImageSize, MetaDataParams metaData);
		}

		[System.Security.SuppressUnmanagedCodeSecurity]
		private unsafe static class WebP_64
		{

			/// Return Type: int
			///iData: uint8_t*
			///iData_size: uint32_t->unsigned int
			///oWidth: int*
			///oHeight: int*
			[DllImport("WebP_x64.dll", EntryPoint = "WebPGetDimensions")]
			[return: MarshalAs(UnmanagedType.I1)]
			public static unsafe extern bool WebPGetDimensions(byte* data, UIntPtr dataSize, out int width, out int height);

			/// Return Type: void
			///mem: void*
			[DllImport("WebP_x64.dll", EntryPoint = "WebPFreeMemory")]
			public static extern void WebPFreeMemory(IntPtr mem);

			/// Return Type: int
			///iBitmap: void*
			///iBitmapSize: int
			///iStride: int
			///iWebPFile: uint8_t*
			///iWebPFileSize: uint32_t->unsigned int
			[DllImport("WebP_x64.dll", EntryPoint = "WebPLoad")]
			public static unsafe extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, ref byte* outData, int outSize, int outStride);

			/// Return Type: int
			///output: void**
			///outputSize: uint32_t->unsigned int
			///iBitmap: void*
			///iBitmapSize: size_t->unsigned int
			///iWidth: int
			///iHeight: int
			///iStride: int
			///iQuality: float
			///iMethod: int
			///iSharpness: int
			///iPreset: WebPPreset->Anonymous_0f2b80ba_5d6f_409d_9e5a_ef9696863a72
			[DllImport("WebP_x64.dll", EntryPoint = "WebPSave")]
			public static unsafe extern WebPEncodingError WebPSave(ref IntPtr output, out uint outputSize, IntPtr scan0, int iWidth, int iHeight, int iStride, EncodeParams parameters, WebPReportProgress callback);

			[DllImport("WebP_x64.dll", EntryPoint = "GetMetaDataSize")]
			public static unsafe extern void GetMetaDataSize(byte* iData, UIntPtr iDataSize, MetaDataType type, out uint metaDataSize);

			[DllImport("WebP_x64.dll", EntryPoint = "ExtractMetaData")]
			public static unsafe extern void ExtractMetaData(byte* iData, UIntPtr iDataSize, byte* metaDataBytes, uint metaDataSize, MetaDataType type);


			[DllImportAttribute("WebP_x64.dll", EntryPoint = "SetMetaData")]
			public static unsafe extern WebPMuxError SetMetaData(byte* image, UIntPtr imageSize, ref IntPtr outImage, ref UIntPtr outImageSize, MetaDataParams metaData);
		}

		private static bool Is64Bit()
		{
			return (IntPtr.Size == 8);
		}

		/// <summary>
		/// Frees the unmanaged memory allocates with <see cref="WebPAllocateMemory"/>.
		/// </summary>
		/// <param name="memory">The memory to free.</param>
		private static void WebPFreeMemory(IntPtr memory)
		{
			if (Is64Bit())
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
		/// <returns>Zero on success, -1 on failure.</returns>
		public static unsafe bool WebPGetDimensions(byte[] data, uint dataSize, out int width, out int height)
		{
			fixed (byte* ptr = data)
			{
				if (Is64Bit())
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
		/// The WepP load function.
		/// </summary>
		/// <param name="data">The input image data, this must be allocated with <see cref="WebPAllocateMemory"/>.</param>
		/// <param name="dataSize">Size of the data.</param>
		/// <param name="width">The width 0f the resulting image.</param>
		/// <param name="outputStride">The height of the resulting image.</param>
		/// <param name="outPtr">The output byte array.</param>
		/// <returns>True on success; false on failure.</returns>
		public static unsafe VP8StatusCode WebPLoad(byte[] data, uint dataSize, byte* outPtr, int outputSize, int outputStride)
		{
			fixed (byte* ptr = data)
			{ 
				if (Is64Bit())
				{
					return WebP_64.WebPLoad(ptr, (UIntPtr)dataSize, ref outPtr, outputSize, outputStride);
				}
				else
				{
				   return WebP_32.WebPLoad(ptr, (UIntPtr)dataSize, ref outPtr, outputSize, outputStride);
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
		public static void WebPSave(out byte[] output, IntPtr scan0, int width, int height, long stride, EncodeParams parameters, WebPReportProgress callback)
		{
			if (width > WebPMaxDimension || height > WebPMaxDimension)
				throw new FormatException(Resources.InvalidImageDimensions);


			WebPEncodingError retVal = WebPEncodingError.Ok;
			IntPtr outPtr = IntPtr.Zero;
			uint dataSize = 0;
			output = null;

			try
			{
				
				if (Is64Bit())
				{
					retVal = WebP_64.WebPSave(ref outPtr, out dataSize, scan0, width, height, (int)stride, parameters, callback);
				}
				else
				{
					retVal = WebP_32.WebPSave(ref outPtr, out dataSize, scan0, width, height, (int)stride, parameters, callback);
				}

				if (retVal == WebPEncodingError.Ok)
				{
					output = new byte[dataSize];
					Marshal.Copy(outPtr, output, 0, output.Length);
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
				if (outPtr != IntPtr.Zero)
				{
					WebPFreeMemory(outPtr);
					outPtr = IntPtr.Zero;
				}
			}
		}

		public static unsafe void GetMetaDataSize(byte[] data, uint dataSize, MetaDataType type, out uint metaDataSize)
		{
			metaDataSize = 0;
			fixed (byte* ptr = data)
			{
				if (Is64Bit())
				{
					WebP_64.GetMetaDataSize(ptr, (UIntPtr)dataSize, type, out metaDataSize);
				}
				else
				{
					WebP_32.GetMetaDataSize(ptr, (UIntPtr)dataSize, type, out metaDataSize);
				}
			}
		}

		public static unsafe void ExtractMetadata(byte[] data, uint dataSize, MetaDataType type, byte[] outData, uint outSize)
		{
			fixed (byte* ptr = data, outPtr = outData)
			{
				if (Is64Bit())
				{
					WebP_64.ExtractMetaData(ptr, (UIntPtr)dataSize, outPtr, outSize, type);
				}
				else
				{
					WebP_32.ExtractMetaData(ptr, (UIntPtr)dataSize, outPtr, outSize, type);
				}
			}
		}

		public static unsafe void SetMetaData(byte[] data, uint dataSize, out byte[] outImage, MetaDataParams metaData)
		{
			outImage = null;

			WebPMuxError error = WebPMuxError.Ok;

			fixed (byte* ptr = data)
			{
				IntPtr outPtr = IntPtr.Zero; 
				UIntPtr outSize = UIntPtr.Zero;

				if (Is64Bit())
				{
					error = WebP_64.SetMetaData(ptr, (UIntPtr)dataSize, ref outPtr, ref outSize, metaData);
				}
				else
				{
					error = WebP_32.SetMetaData(ptr, (UIntPtr)dataSize, ref outPtr, ref outSize, metaData);
				}

				if (error == WebPMuxError.Ok)
				{ 
					uint size = outSize.ToUInt32();

					outImage = new byte[size];
					Marshal.Copy(outPtr, outImage, 0, outImage.Length);

					WebPFreeMemory(outPtr);
					outPtr = IntPtr.Zero;
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
		}


	}
}
