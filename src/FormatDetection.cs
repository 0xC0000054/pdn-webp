////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2022 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using PaintDotNet.AppModel;
using System;
using System.Collections.Generic;

#nullable enable

namespace WebPFileType
{
    internal static class FormatDetection
    {
        private static ReadOnlySpan<byte> Gif87aFileSignature => new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 };

        private static ReadOnlySpan<byte> Gif89aFileSignature => new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 };

        private static ReadOnlySpan<byte> JpegFileSignature => new byte[] { 0xff, 0xd8, 0xff };

        private static ReadOnlySpan<byte> PngFileSignature => new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

        private static ReadOnlySpan<byte> RiffSignature => new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' };

        private static ReadOnlySpan<byte> RiffWebPSignature => new byte[] { (byte)'W', (byte)'E', (byte)'B', (byte)'P' };

        private static ReadOnlySpan<byte> TiffBigEndianFileSignature => new byte[] { 0x4d, 0x4d, 0x00, 0x2a };

        private static ReadOnlySpan<byte> TiffLittleEndianFileSignature => new byte[] { 0x49, 0x49, 0x2a, 0x00 };

        /// <summary>
        /// Determines whether the specified file has a WebP file signature.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        ///   <see langword="true"/> if the file has a WebP file signature; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasWebPFileSignature(ReadOnlySpan<byte> file)
        {
            bool result = false;

            // The WebP file header is 12 bytes long and has the following layout:
            // Bytes 0-3: the ASCII characters 'R' 'I' 'F' 'F'
            // Bytes 4-7: the file size as an unsigned 32-bit integer
            // Bytes 8-11: the ASCII characters 'W' 'E' 'B' 'P'
            if (file.Length >= 12)
            {
                result = RiffSignature.SequenceEqual(file.Slice(0, 4))
                      && RiffWebPSignature.SequenceEqual(file.Slice(8, 4));
            }

            return result;
        }

        /// <summary>
        /// Attempts to get an <see cref="IFileTypeInfo"/> from the file signature.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        ///   An <see cref="IFileTypeInfo"/> instance if the file has the signature of a recognized image format;
        ///   otherwise, <see langword="null"/>.
        /// </returns>
        /// <remarks>
        /// Some applications may save other common image formats (e.g. JPEG or PNG) with a .webp file extension.
        /// </remarks>
        internal static IFileTypeInfo? TryGetFileTypeInfo(ReadOnlySpan<byte> file, IServiceProvider? serviceProvider)
        {
            string name = TryGetFileTypeName(file);

            IFileTypeInfo? fileTypeInfo = null;

            if (string.IsNullOrEmpty(name))
            {
                IFileTypesService? fileTypesService = serviceProvider?.GetService<IFileTypesService>();

                if (fileTypesService != null)
                {
                    foreach (IFileTypeInfo item in fileTypesService.FileTypes)
                    {
                        if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                            && item.Options.SupportsLoading)
                        {
                            fileTypeInfo = item;
                            break;
                        }
                    }
                }
            }

            return fileTypeInfo;
        }

        private static string TryGetFileTypeName(ReadOnlySpan<byte> file)
        {
            string name = string.Empty;

            if (FileSignatureMatches(file, JpegFileSignature))
            {
                name = "JPEG";
            }
            else if (FileSignatureMatches(file, PngFileSignature))
            {
                name = "PNG";
            }
            else if (IsGifFileSignature(file))
            {
                name = "GIF";
            }
            else if (IsTiffFileSignature(file))
            {
                name = "TIFF";
            }

            return name;
        }

        private static bool FileSignatureMatches(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature)
            => data.Length >= signature.Length && data.Slice(0, signature.Length).SequenceEqual(signature);

        private static bool IsGifFileSignature(ReadOnlySpan<byte> data)
        {
            bool result = false;

            if (data.Length >= Gif87aFileSignature.Length)
            {
                ReadOnlySpan<byte> bytes = data.Slice(0, Gif87aFileSignature.Length);

                result = bytes.SequenceEqual(Gif87aFileSignature)
                      || bytes.SequenceEqual(Gif89aFileSignature);
            }

            return result;
        }

        private static bool IsTiffFileSignature(ReadOnlySpan<byte> data)
        {
            bool result = false;

            if (data.Length >= TiffBigEndianFileSignature.Length)
            {
                ReadOnlySpan<byte> bytes = data.Slice(0, TiffBigEndianFileSignature.Length);

                result = bytes.SequenceEqual(TiffBigEndianFileSignature)
                      || bytes.SequenceEqual(TiffLittleEndianFileSignature);
            }

            return result;
        }
    }
}
