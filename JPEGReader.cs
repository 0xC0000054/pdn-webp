using System;
using System.Text;

namespace WebPFileType
{
    internal static class JPEGReader
    {
        private static class JpegMarkers
        {
            internal const ushort StartOfImage = 0xFFD8;
            internal const ushort EndOfImage = 0xFFD9;
            internal const ushort StartOfScan = 0xFFDA;
            internal const ushort App1 = 0xFFE1;
        }

        private const int EXIFSignatureLength = 6;
        private const int EXIFSegmentHeaderLength = sizeof(ushort) + EXIFSignatureLength;

        private static ushort ReadUInt16BigEndian(byte[] buffer, int startIndex)
        {
            return (ushort)((buffer[startIndex] << 8) | buffer[startIndex + 1]);
        }

        /// <summary>
        /// Extracts the EXIF data from a JPEG image.
        /// </summary>
        /// <param name="jpegBytes">The JPEG image bytes.</param>
        /// <returns>The extracted EXIF data, or null.</returns>
        internal static byte[] ExtractEXIF(byte[] jpegBytes)
        {
            try
            {
                if (jpegBytes.Length > 2)
                {
                    ushort marker = ReadUInt16BigEndian(jpegBytes, 0);

                    // Check the file signature.
                    if (marker == JpegMarkers.StartOfImage)
                    {
                        int index = 2;
                        int length = jpegBytes.Length;

                        while (index < length)
                        {
                            marker = ReadUInt16BigEndian(jpegBytes, index);
                            if (marker == 0xFFFF)
                            {
                                // Skip the first padding byte and read the marker again.
                                index++;
                                continue;
                            }
                            else
                            {
                                index += 2;
                            }

                            if (marker == JpegMarkers.StartOfScan || marker == JpegMarkers.EndOfImage)
                            {
                                // The application data segments always come before these markers.
                                break;
                            }

                            // The segment length field includes its own length in the total.
                            // The index is not incremented after reading it to avoid having to subtract
                            // 2 bytes from the length when skipping a segment.
                            ushort segmentLength = ReadUInt16BigEndian(jpegBytes, index);

                            if (marker == JpegMarkers.App1 && segmentLength >= EXIFSegmentHeaderLength)
                            {
                                string sig = Encoding.UTF8.GetString(jpegBytes, index + 2, EXIFSignatureLength);
                                if (sig.Equals("Exif\0\0", StringComparison.Ordinal))
                                {
                                    int exifDataSize = segmentLength - EXIFSegmentHeaderLength;
                                    byte[] exifData = null;

                                    if (exifDataSize > 0)
                                    {
                                        exifData = new byte[exifDataSize];
                                        Buffer.BlockCopy(jpegBytes, index + EXIFSegmentHeaderLength, exifData, 0, exifDataSize);
                                    }

                                    return exifData;
                                }
                            }

                            index += segmentLength;
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
            }

            return null;
        }
    }
}
