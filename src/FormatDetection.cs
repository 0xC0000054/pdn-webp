////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2026 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using PaintDotNet.FileTypes;
using System;

#nullable enable

namespace WebPFileType
{
    internal static class FormatDetection
    {
        private static ReadOnlySpan<byte> BmpFileSignature => [0x42, 0x4D];

        private static ReadOnlySpan<byte> Gif87aFileSignature => [0x47, 0x49, 0x46, 0x38, 0x37, 0x61];

        private static ReadOnlySpan<byte> Gif89aFileSignature => [0x47, 0x49, 0x46, 0x38, 0x39, 0x61];

        private static ReadOnlySpan<byte> JpegFileSignature => [0xff, 0xd8, 0xff];

        private static ReadOnlySpan<byte> PngFileSignature => [137, 80, 78, 71, 13, 10, 26, 10];

        private static ReadOnlySpan<byte> RiffSignature => [(byte)'R', (byte)'I', (byte)'F', (byte)'F'];

        private static ReadOnlySpan<byte> RiffWebPSignature => [(byte)'W', (byte)'E', (byte)'B', (byte)'P'];

        private static ReadOnlySpan<byte> TiffBigEndianFileSignature => [0x4d, 0x4d, 0x00, 0x2a];

        private static ReadOnlySpan<byte> TiffLittleEndianFileSignature => [0x49, 0x49, 0x2a, 0x00];

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
            string? ext = TryGetFileTypeExtension(file);
            if (string.IsNullOrEmpty(ext))
            {
                return null;
            }

            IFileTypesService? fileTypesService = serviceProvider?.GetService<IFileTypesService>();
            if (fileTypesService != null)
            {
                return fileTypesService.FindFileTypeForLoadingExtension(ext);
            }

            return null;
        }

        private static string? TryGetFileTypeExtension(ReadOnlySpan<byte> file)
        {
            string? ext = null;

            if (FileSignatureMatches(file, JpegFileSignature))
            {
                ext = ".jpg";
            }
            else if (FileSignatureMatches(file, PngFileSignature))
            {
                ext = ".png";
            }
            else if (FileSignatureMatches(file, BmpFileSignature))
            {
                ext = ".bmp";
            }
            else if (IsGifFileSignature(file))
            {
                ext = ".gif";
            }
            else if (IsTiffFileSignature(file))
            {
                ext = ".tif";
            }

            return ext;
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
