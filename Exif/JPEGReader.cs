////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2019 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace WebPFileType.Exif
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

        /// <summary>
        /// Extracts the EXIF data from a JPEG image.
        /// </summary>
        /// <param name="stream">The JPEG image stream.</param>
        /// <returns>The extracted EXIF data, or null.</returns>
        internal static byte[] ExtractEXIF(Stream stream)
        {
            stream.Position = 0;
            try
            {
                using (EndianBinaryReader reader = new EndianBinaryReader(stream, Endianess.Big, true))
                {
                    ushort marker = reader.ReadUInt16();

                    // Check the file signature.
                    if (marker == JpegMarkers.StartOfImage)
                    {
                        while (reader.Position < reader.Length)
                        {
                            marker = reader.ReadUInt16();
                            if (marker == 0xFFFF)
                            {
                                // Skip the first padding byte and read the marker again.
                                reader.Position++;
                                continue;
                            }

                            if (marker == JpegMarkers.StartOfScan || marker == JpegMarkers.EndOfImage)
                            {
                                // The application data segments always come before these markers.
                                break;
                            }

                            // The segment length field includes its own length in the total.
                            int segmentLength = reader.ReadUInt16() - sizeof(ushort);

                            if (marker == JpegMarkers.App1 && segmentLength >= EXIFSignatureLength)
                            {
                                string sig = reader.ReadAsciiString(EXIFSignatureLength);
                                if (sig.Equals("Exif\0\0", StringComparison.Ordinal))
                                {
                                    int exifDataSize = segmentLength - EXIFSignatureLength;
                                    byte[] exifData = null;

                                    if (exifDataSize > 0)
                                    {
                                        exifData = reader.ReadBytes(exifDataSize);
                                    }

                                    return exifData;
                                }
                                else
                                {
                                    segmentLength -= EXIFSignatureLength;
                                }
                            }

                            reader.Position += segmentLength;
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
            }

            return null;
        }
    }
}
