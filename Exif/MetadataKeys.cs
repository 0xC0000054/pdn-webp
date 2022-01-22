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

namespace WebPFileType.Exif
{
    internal static class MetadataKeys
    {
        // Generated from https://www.exiv2.org/tags.html

        internal static class Image
        {
            /// <summary>
            /// Image.ProcessingSoftware
            /// </summary>
            /// <remarks>
            /// The name and version of the software used to post-process the picture.
            /// </remarks>
            internal static readonly MetadataKey ProcessingSoftware = new(MetadataSection.Image, 11);

            /// <summary>
            /// Image.NewSubfileType
            /// </summary>
            /// <remarks>
            /// A general indication of the kind of data contained in this subfile.
            /// </remarks>
            internal static readonly MetadataKey NewSubfileType = new(MetadataSection.Image, 254);

            /// <summary>
            /// Image.SubfileType
            /// </summary>
            /// <remarks>
            /// A general indication of the kind of data contained in this subfile. This field is deprecated. The NewSubfileType field should be used instead.
            /// </remarks>
            internal static readonly MetadataKey SubfileType = new(MetadataSection.Image, 255);

            /// <summary>
            /// Image.ImageWidth
            /// </summary>
            /// <remarks>
            /// The number of columns of image data, equal to the number of pixels per row. In JPEG compressed data a JPEG marker is used instead of this tag.
            /// </remarks>
            internal static readonly MetadataKey ImageWidth = new(MetadataSection.Image, 256);

            /// <summary>
            /// Image.ImageLength
            /// </summary>
            /// <remarks>
            /// The number of rows of image data. In JPEG compressed data a JPEG marker is used instead of this tag.
            /// </remarks>
            internal static readonly MetadataKey ImageLength = new(MetadataSection.Image, 257);

            /// <summary>
            /// Image.BitsPerSample
            /// </summary>
            /// <remarks>
            /// The number of bits per image component. In this standard each component of the image is 8 bits, so the value for this tag is 8. See also
            /// 'SamplesPerPixel'. In JPEG compressed data a JPEG marker is used instead of this tag.
            /// </remarks>
            internal static readonly MetadataKey BitsPerSample = new(MetadataSection.Image, 258);

            /// <summary>
            /// Image.Compression
            /// </summary>
            /// <remarks>
            /// The compression scheme used for the image data. When a primary image is JPEG compressed, this designation is not necessary and is omitted. When
            /// thumbnails use JPEG compression, this tag value is set to 6.
            /// </remarks>
            internal static readonly MetadataKey Compression = new(MetadataSection.Image, 259);

            /// <summary>
            /// Image.PhotometricInterpretation
            /// </summary>
            /// <remarks>
            /// The pixel composition. In JPEG compressed data a JPEG marker is used instead of this tag.
            /// </remarks>
            internal static readonly MetadataKey PhotometricInterpretation = new(MetadataSection.Image, 262);

            /// <summary>
            /// Image.Thresholding
            /// </summary>
            /// <remarks>
            /// For black and white TIFF files that represent shades of gray, the technique used to convert from gray to black and white pixels.
            /// </remarks>
            internal static readonly MetadataKey Thresholding = new(MetadataSection.Image, 263);

            /// <summary>
            /// Image.CellWidth
            /// </summary>
            /// <remarks>
            /// The width of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file.
            /// </remarks>
            internal static readonly MetadataKey CellWidth = new(MetadataSection.Image, 264);

            /// <summary>
            /// Image.CellLength
            /// </summary>
            /// <remarks>
            /// The length of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file.
            /// </remarks>
            internal static readonly MetadataKey CellLength = new(MetadataSection.Image, 265);

            /// <summary>
            /// Image.FillOrder
            /// </summary>
            /// <remarks>
            /// The logical order of bits within a byte
            /// </remarks>
            internal static readonly MetadataKey FillOrder = new(MetadataSection.Image, 266);

            /// <summary>
            /// Image.DocumentName
            /// </summary>
            /// <remarks>
            /// The name of the document from which this image was scanned
            /// </remarks>
            internal static readonly MetadataKey DocumentName = new(MetadataSection.Image, 269);

            /// <summary>
            /// Image.ImageDescription
            /// </summary>
            /// <remarks>
            /// A character string giving the title of the image. It may be a comment such as "1988 company picnic" or the like. Two-bytes character codes cannot be
            /// used. When a 2-bytes code is necessary, the Exif Private tag 'UserComment' is to be used.
            /// </remarks>
            internal static readonly MetadataKey ImageDescription = new(MetadataSection.Image, 270);

            /// <summary>
            /// Image.Make
            /// </summary>
            /// <remarks>
            /// The manufacturer of the recording equipment. This is the manufacturer of the DSC, scanner, video digitizer or other equipment that generated the
            /// image. When the field is left blank, it is treated as unknown.
            /// </remarks>
            internal static readonly MetadataKey Make = new(MetadataSection.Image, 271);

            /// <summary>
            /// Image.Model
            /// </summary>
            /// <remarks>
            /// The model name or model number of the equipment. This is the model name or number of the DSC, scanner, video digitizer or other equipment that
            /// generated the image. When the field is left blank, it is treated as unknown.
            /// </remarks>
            internal static readonly MetadataKey Model = new(MetadataSection.Image, 272);

            /// <summary>
            /// Image.StripOffsets
            /// </summary>
            /// <remarks>
            /// For each strip, the byte offset of that strip. It is recommended that this be selected so the number of strip bytes does not exceed 64 Kbytes. With
            /// JPEG compressed data this designation is not needed and is omitted. See also 'RowsPerStrip' and 'StripByteCounts'.
            /// </remarks>
            internal static readonly MetadataKey StripOffsets = new(MetadataSection.Image, 273);

            /// <summary>
            /// Image.Orientation
            /// </summary>
            /// <remarks>
            /// The image orientation viewed in terms of rows and columns.
            /// </remarks>
            internal static readonly MetadataKey Orientation = new(MetadataSection.Image, 274);

            /// <summary>
            /// Image.SamplesPerPixel
            /// </summary>
            /// <remarks>
            /// The number of components per pixel. Since this standard applies to RGB and YCbCr images, the value set for this tag is 3. In JPEG compressed data a
            /// JPEG marker is used instead of this tag.
            /// </remarks>
            internal static readonly MetadataKey SamplesPerPixel = new(MetadataSection.Image, 277);

            /// <summary>
            /// Image.RowsPerStrip
            /// </summary>
            /// <remarks>
            /// The number of rows per strip. This is the number of rows in the image of one strip when an image is divided into strips. With JPEG compressed data
            /// this designation is not needed and is omitted. See also 'StripOffsets' and 'StripByteCounts'.
            /// </remarks>
            internal static readonly MetadataKey RowsPerStrip = new(MetadataSection.Image, 278);

            /// <summary>
            /// Image.StripByteCounts
            /// </summary>
            /// <remarks>
            /// The total number of bytes in each strip. With JPEG compressed data this designation is not needed and is omitted.
            /// </remarks>
            internal static readonly MetadataKey StripByteCounts = new(MetadataSection.Image, 279);

            /// <summary>
            /// Image.XResolution
            /// </summary>
            /// <remarks>
            /// The number of pixels per 'ResolutionUnit' in the 'ImageWidth' direction. When the image resolution is unknown, 72 [dpi] is designated.
            /// </remarks>
            internal static readonly MetadataKey XResolution = new(MetadataSection.Image, 282);

            /// <summary>
            /// Image.YResolution
            /// </summary>
            /// <remarks>
            /// The number of pixels per 'ResolutionUnit' in the 'ImageLength' direction. The same value as 'XResolution' is designated.
            /// </remarks>
            internal static readonly MetadataKey YResolution = new(MetadataSection.Image, 283);

            /// <summary>
            /// Image.PlanarConfiguration
            /// </summary>
            /// <remarks>
            /// Indicates whether pixel components are recorded in a chunky or planar format. In JPEG compressed files a JPEG marker is used instead of this tag. If
            /// this field does not exist, the TIFF default of 1 (chunky) is assumed.
            /// </remarks>
            internal static readonly MetadataKey PlanarConfiguration = new(MetadataSection.Image, 284);

            /// <summary>
            /// Image.GrayResponseUnit
            /// </summary>
            /// <remarks>
            /// The precision of the information contained in the GrayResponseCurve.
            /// </remarks>
            internal static readonly MetadataKey GrayResponseUnit = new(MetadataSection.Image, 290);

            /// <summary>
            /// Image.GrayResponseCurve
            /// </summary>
            /// <remarks>
            /// For grayscale data, the optical density of each possible pixel value.
            /// </remarks>
            internal static readonly MetadataKey GrayResponseCurve = new(MetadataSection.Image, 291);

            /// <summary>
            /// Image.T4Options
            /// </summary>
            /// <remarks>
            /// T.4-encoding options.
            /// </remarks>
            internal static readonly MetadataKey T4Options = new(MetadataSection.Image, 292);

            /// <summary>
            /// Image.T6Options
            /// </summary>
            /// <remarks>
            /// T.6-encoding options.
            /// </remarks>
            internal static readonly MetadataKey T6Options = new(MetadataSection.Image, 293);

            /// <summary>
            /// Image.ResolutionUnit
            /// </summary>
            /// <remarks>
            /// The unit for measuring 'XResolution' and 'YResolution'. The same unit is used for both 'XResolution' and 'YResolution'. If the image resolution is
            /// unknown, 2 (inches) is designated.
            /// </remarks>
            internal static readonly MetadataKey ResolutionUnit = new(MetadataSection.Image, 296);

            /// <summary>
            /// Image.PageNumber
            /// </summary>
            /// <remarks>
            /// The page number of the page from which this image was scanned.
            /// </remarks>
            internal static readonly MetadataKey PageNumber = new(MetadataSection.Image, 297);

            /// <summary>
            /// Image.TransferFunction
            /// </summary>
            /// <remarks>
            /// A transfer function for the image, described in tabular style. Normally this tag is not necessary, since color space is specified in the color space
            /// information tag ('ColorSpace').
            /// </remarks>
            internal static readonly MetadataKey TransferFunction = new(MetadataSection.Image, 301);

            /// <summary>
            /// Image.Software
            /// </summary>
            /// <remarks>
            /// This tag records the name and version of the software or firmware of the camera or image input device used to generate the image. The detailed format
            /// is not specified, but it is recommended that the example shown below be followed. When the field is left blank, it is treated as unknown.
            /// </remarks>
            internal static readonly MetadataKey Software = new(MetadataSection.Image, 305);

            /// <summary>
            /// Image.DateTime
            /// </summary>
            /// <remarks>
            /// The date and time of image creation. In Exif standard, it is the date and time the file was changed.
            /// </remarks>
            internal static readonly MetadataKey DateTime = new(MetadataSection.Image, 306);

            /// <summary>
            /// Image.Artist
            /// </summary>
            /// <remarks>
            /// This tag records the name of the camera owner, photographer or image creator. The detailed format is not specified, but it is recommended that the
            /// information be written as in the example below for ease of Interoperability. When the field is left blank, it is treated as unknown. Ex.) "Camera
            /// owner, John Smith; Photographer, Michael Brown; Image creator, Ken James"
            /// </remarks>
            internal static readonly MetadataKey Artist = new(MetadataSection.Image, 315);

            /// <summary>
            /// Image.HostComputer
            /// </summary>
            /// <remarks>
            /// This tag records information about the host computer used to generate the image.
            /// </remarks>
            internal static readonly MetadataKey HostComputer = new(MetadataSection.Image, 316);

            /// <summary>
            /// Image.Predictor
            /// </summary>
            /// <remarks>
            /// A predictor is a mathematical operator that is applied to the image data before an encoding scheme is applied.
            /// </remarks>
            internal static readonly MetadataKey Predictor = new(MetadataSection.Image, 317);

            /// <summary>
            /// Image.WhitePoint
            /// </summary>
            /// <remarks>
            /// The chromaticity of the white point of the image. Normally this tag is not necessary, since color space is specified in the colorspace information tag
            /// ('ColorSpace').
            /// </remarks>
            internal static readonly MetadataKey WhitePoint = new(MetadataSection.Image, 318);

            /// <summary>
            /// Image.PrimaryChromaticities
            /// </summary>
            /// <remarks>
            /// The chromaticity of the three primary colors of the image. Normally this tag is not necessary, since colorspace is specified in the colorspace
            /// information tag ('ColorSpace').
            /// </remarks>
            internal static readonly MetadataKey PrimaryChromaticities = new(MetadataSection.Image, 319);

            /// <summary>
            /// Image.ColorMap
            /// </summary>
            /// <remarks>
            /// A color map for palette color images. This field defines a Red-Green-Blue color map (often called a lookup table) for palette-color images. In a
            /// palette-color image, a pixel value is used to index into an RGB lookup table.
            /// </remarks>
            internal static readonly MetadataKey ColorMap = new(MetadataSection.Image, 320);

            /// <summary>
            /// Image.HalftoneHints
            /// </summary>
            /// <remarks>
            /// The purpose of the HalftoneHints field is to convey to the halftone function the range of gray levels within a colorimetrically-specified image that
            /// should retain tonal detail.
            /// </remarks>
            internal static readonly MetadataKey HalftoneHints = new(MetadataSection.Image, 321);

            /// <summary>
            /// Image.TileWidth
            /// </summary>
            /// <remarks>
            /// The tile width in pixels. This is the number of columns in each tile.
            /// </remarks>
            internal static readonly MetadataKey TileWidth = new(MetadataSection.Image, 322);

            /// <summary>
            /// Image.TileLength
            /// </summary>
            /// <remarks>
            /// The tile length (height) in pixels. This is the number of rows in each tile.
            /// </remarks>
            internal static readonly MetadataKey TileLength = new(MetadataSection.Image, 323);

            /// <summary>
            /// Image.TileOffsets
            /// </summary>
            /// <remarks>
            /// For each tile, the byte offset of that tile, as compressed and stored on disk. The offset is specified with respect to the beginning of the TIFF file.
            /// Note that this implies that each tile has a location independent of the locations of other tiles.
            /// </remarks>
            internal static readonly MetadataKey TileOffsets = new(MetadataSection.Image, 324);

            /// <summary>
            /// Image.TileByteCounts
            /// </summary>
            /// <remarks>
            /// For each tile, the number of (compressed) bytes in that tile. See TileOffsets for a description of how the byte counts are ordered.
            /// </remarks>
            internal static readonly MetadataKey TileByteCounts = new(MetadataSection.Image, 325);

            /// <summary>
            /// Image.SubIFDs
            /// </summary>
            /// <remarks>
            /// Defined by Adobe Corporation to enable TIFF Trees within a TIFF file.
            /// </remarks>
            internal static readonly MetadataKey SubIFDs = new(MetadataSection.Image, 330);

            /// <summary>
            /// Image.InkSet
            /// </summary>
            /// <remarks>
            /// The set of inks used in a separated (PhotometricInterpretation=5) image.
            /// </remarks>
            internal static readonly MetadataKey InkSet = new(MetadataSection.Image, 332);

            /// <summary>
            /// Image.InkNames
            /// </summary>
            /// <remarks>
            /// The name of each ink used in a separated (PhotometricInterpretation=5) image.
            /// </remarks>
            internal static readonly MetadataKey InkNames = new(MetadataSection.Image, 333);

            /// <summary>
            /// Image.NumberOfInks
            /// </summary>
            /// <remarks>
            /// The number of inks. Usually equal to SamplesPerPixel, unless there are extra samples.
            /// </remarks>
            internal static readonly MetadataKey NumberOfInks = new(MetadataSection.Image, 334);

            /// <summary>
            /// Image.DotRange
            /// </summary>
            /// <remarks>
            /// The component values that correspond to a 0% dot and 100% dot.
            /// </remarks>
            internal static readonly MetadataKey DotRange = new(MetadataSection.Image, 336);

            /// <summary>
            /// Image.TargetPrinter
            /// </summary>
            /// <remarks>
            /// A description of the printing environment for which this separation is intended.
            /// </remarks>
            internal static readonly MetadataKey TargetPrinter = new(MetadataSection.Image, 337);

            /// <summary>
            /// Image.ExtraSamples
            /// </summary>
            /// <remarks>
            /// Specifies that each pixel has m extra components whose interpretation is defined by one of the values listed below.
            /// </remarks>
            internal static readonly MetadataKey ExtraSamples = new(MetadataSection.Image, 338);

            /// <summary>
            /// Image.SampleFormat
            /// </summary>
            /// <remarks>
            /// This field specifies how to interpret each data sample in a pixel.
            /// </remarks>
            internal static readonly MetadataKey SampleFormat = new(MetadataSection.Image, 339);

            /// <summary>
            /// Image.SMinSampleValue
            /// </summary>
            /// <remarks>
            /// This field specifies the minimum sample value.
            /// </remarks>
            internal static readonly MetadataKey SMinSampleValue = new(MetadataSection.Image, 340);

            /// <summary>
            /// Image.SMaxSampleValue
            /// </summary>
            /// <remarks>
            /// This field specifies the maximum sample value.
            /// </remarks>
            internal static readonly MetadataKey SMaxSampleValue = new(MetadataSection.Image, 341);

            /// <summary>
            /// Image.TransferRange
            /// </summary>
            /// <remarks>
            /// Expands the range of the TransferFunction
            /// </remarks>
            internal static readonly MetadataKey TransferRange = new(MetadataSection.Image, 342);

            /// <summary>
            /// Image.ClipPath
            /// </summary>
            /// <remarks>
            /// A TIFF ClipPath is intended to mirror the essentials of PostScript's path creation functionality.
            /// </remarks>
            internal static readonly MetadataKey ClipPath = new(MetadataSection.Image, 343);

            /// <summary>
            /// Image.XClipPathUnits
            /// </summary>
            /// <remarks>
            /// The number of units that span the width of the image, in terms of integer ClipPath coordinates.
            /// </remarks>
            internal static readonly MetadataKey XClipPathUnits = new(MetadataSection.Image, 344);

            /// <summary>
            /// Image.YClipPathUnits
            /// </summary>
            /// <remarks>
            /// The number of units that span the height of the image, in terms of integer ClipPath coordinates.
            /// </remarks>
            internal static readonly MetadataKey YClipPathUnits = new(MetadataSection.Image, 345);

            /// <summary>
            /// Image.Indexed
            /// </summary>
            /// <remarks>
            /// Indexed images are images where the 'pixels' do not represent color values, but rather an index (usually 8-bit) into a separate color table, the
            /// ColorMap.
            /// </remarks>
            internal static readonly MetadataKey Indexed = new(MetadataSection.Image, 346);

            /// <summary>
            /// Image.JPEGTables
            /// </summary>
            /// <remarks>
            /// This optional tag may be used to encode the JPEG quantization and Huffman tables for subsequent use by the JPEG decompression process.
            /// </remarks>
            internal static readonly MetadataKey JPEGTables = new(MetadataSection.Image, 347);

            /// <summary>
            /// Image.OPIProxy
            /// </summary>
            /// <remarks>
            /// OPIProxy gives information concerning whether this image is a low-resolution proxy of a high-resolution image (Adobe OPI).
            /// </remarks>
            internal static readonly MetadataKey OPIProxy = new(MetadataSection.Image, 351);

            /// <summary>
            /// Image.JPEGProc
            /// </summary>
            /// <remarks>
            /// This field indicates the process used to produce the compressed data
            /// </remarks>
            internal static readonly MetadataKey JPEGProc = new(MetadataSection.Image, 512);

            /// <summary>
            /// Image.JPEGInterchangeFormat
            /// </summary>
            /// <remarks>
            /// The offset to the start byte (SOI) of JPEG compressed thumbnail data. This is not used for primary image JPEG data.
            /// </remarks>
            internal static readonly MetadataKey JPEGInterchangeFormat = new(MetadataSection.Image, 513);

            /// <summary>
            /// Image.JPEGInterchangeFormatLength
            /// </summary>
            /// <remarks>
            /// The number of bytes of JPEG compressed thumbnail data. This is not used for primary image JPEG data. JPEG thumbnails are not divided but are recorded
            /// as a continuous JPEG bitstream from SOI to EOI. Appn and COM markers should not be recorded. Compressed thumbnails must be recorded in no more than 64
            /// Kbytes, including all other data to be recorded in APP1.
            /// </remarks>
            internal static readonly MetadataKey JPEGInterchangeFormatLength = new(MetadataSection.Image, 514);

            /// <summary>
            /// Image.JPEGRestartInterval
            /// </summary>
            /// <remarks>
            /// This Field indicates the length of the restart interval used in the compressed image data.
            /// </remarks>
            internal static readonly MetadataKey JPEGRestartInterval = new(MetadataSection.Image, 515);

            /// <summary>
            /// Image.JPEGLosslessPredictors
            /// </summary>
            /// <remarks>
            /// This Field points to a list of lossless predictor-selection values, one per component.
            /// </remarks>
            internal static readonly MetadataKey JPEGLosslessPredictors = new(MetadataSection.Image, 517);

            /// <summary>
            /// Image.JPEGPointTransforms
            /// </summary>
            /// <remarks>
            /// This Field points to a list of point transform values, one per component.
            /// </remarks>
            internal static readonly MetadataKey JPEGPointTransforms = new(MetadataSection.Image, 518);

            /// <summary>
            /// Image.JPEGQTables
            /// </summary>
            /// <remarks>
            /// This Field points to a list of offsets to the quantization tables, one per component.
            /// </remarks>
            internal static readonly MetadataKey JPEGQTables = new(MetadataSection.Image, 519);

            /// <summary>
            /// Image.JPEGDCTables
            /// </summary>
            /// <remarks>
            /// This Field points to a list of offsets to the DC Huffman tables or the lossless Huffman tables, one per component.
            /// </remarks>
            internal static readonly MetadataKey JPEGDCTables = new(MetadataSection.Image, 520);

            /// <summary>
            /// Image.JPEGACTables
            /// </summary>
            /// <remarks>
            /// This Field points to a list of offsets to the Huffman AC tables, one per component.
            /// </remarks>
            internal static readonly MetadataKey JPEGACTables = new(MetadataSection.Image, 521);

            /// <summary>
            /// Image.YCbCrCoefficients
            /// </summary>
            /// <remarks>
            /// The matrix coefficients for transformation from RGB to YCbCr image data. No default is given in TIFF; but here the value given in Appendix E, "Color
            /// Space Guidelines", is used as the default. The color space is declared in a color space information tag, with the default being the value that gives
            /// the optimal image characteristics Interoperability this condition.
            /// </remarks>
            internal static readonly MetadataKey YCbCrCoefficients = new(MetadataSection.Image, 529);

            /// <summary>
            /// Image.YCbCrSubSampling
            /// </summary>
            /// <remarks>
            /// The sampling ratio of chrominance components in relation to the luminance component. In JPEG compressed data a JPEG marker is used instead of this
            /// tag.
            /// </remarks>
            internal static readonly MetadataKey YCbCrSubSampling = new(MetadataSection.Image, 530);

            /// <summary>
            /// Image.YCbCrPositioning
            /// </summary>
            /// <remarks>
            /// The position of chrominance components in relation to the luminance component. This field is designated only for JPEG compressed data or uncompressed
            /// YCbCr data. The TIFF default is 1 (centered); but when Y:Cb:Cr = 4:2:2 it is recommended in this standard that 2 (co-sited) be used to record data, in
            /// order to improve the image quality when viewed on TV systems. When this field does not exist, the reader shall assume the TIFF default. In the case of
            /// Y:Cb:Cr = 4:2:0, the TIFF default (centered) is recommended. If the reader does not have the capability of supporting both kinds of
            /// 'YCbCrPositioning', it shall follow the TIFF default regardless of the value in this field. It is preferable that readers be able to support both
            /// centered and co-sited positioning.
            /// </remarks>
            internal static readonly MetadataKey YCbCrPositioning = new(MetadataSection.Image, 531);

            /// <summary>
            /// Image.ReferenceBlackWhite
            /// </summary>
            /// <remarks>
            /// The reference black point value and reference white point value. No defaults are given in TIFF, but the values below are given as defaults here. The
            /// color space is declared in a color space information tag, with the default being the value that gives the optimal image characteristics
            /// Interoperability these conditions.
            /// </remarks>
            internal static readonly MetadataKey ReferenceBlackWhite = new(MetadataSection.Image, 532);

            /// <summary>
            /// Image.XMLPacket
            /// </summary>
            /// <remarks>
            /// XMP Metadata (Adobe technote 9-14-02)
            /// </remarks>
            internal static readonly MetadataKey XMLPacket = new(MetadataSection.Image, 700);

            /// <summary>
            /// Image.Rating
            /// </summary>
            /// <remarks>
            /// Rating tag used by Windows
            /// </remarks>
            internal static readonly MetadataKey Rating = new(MetadataSection.Image, 18246);

            /// <summary>
            /// Image.RatingPercent
            /// </summary>
            /// <remarks>
            /// Rating tag used by Windows, value in percent
            /// </remarks>
            internal static readonly MetadataKey RatingPercent = new(MetadataSection.Image, 18249);

            /// <summary>
            /// Image.ImageID
            /// </summary>
            /// <remarks>
            /// ImageID is the full pathname of the original, high-resolution image, or any other identifying string that uniquely identifies the original image
            /// (Adobe OPI).
            /// </remarks>
            internal static readonly MetadataKey ImageID = new(MetadataSection.Image, 32781);

            /// <summary>
            /// Image.CFARepeatPatternDim
            /// </summary>
            /// <remarks>
            /// Contains two values representing the minimum rows and columns to define the repeating patterns of the color filter array
            /// </remarks>
            internal static readonly MetadataKey CFARepeatPatternDim = new(MetadataSection.Image, 33421);

            /// <summary>
            /// Image.CFAPattern
            /// </summary>
            /// <remarks>
            /// Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used. It does not apply to all
            /// sensing methods
            /// </remarks>
            internal static readonly MetadataKey CFAPattern = new(MetadataSection.Image, 33422);

            /// <summary>
            /// Image.BatteryLevel
            /// </summary>
            /// <remarks>
            /// Contains a value of the battery level as a fraction or string
            /// </remarks>
            internal static readonly MetadataKey BatteryLevel = new(MetadataSection.Image, 33423);

            /// <summary>
            /// Image.Copyright
            /// </summary>
            /// <remarks>
            /// Copyright information. In this standard the tag is used to indicate both the photographer and editor copyrights. It is the copyright notice of the
            /// person or organization claiming rights to the image. The Interoperability copyright statement including date and rights should be written in this
            /// field; e.g., "Copyright, John Smith, 19xx. All rights reserved.". In this standard the field records both the photographer and editor copyrights, with
            /// each recorded in a separate part of the statement. When there is a clear distinction between the photographer and editor copyrights, these are to be
            /// written in the order of photographer followed by editor copyright, separated by NULL (in this case since the statement also ends with a NULL, there
            /// are two NULL codes). When only the photographer copyright is given, it is terminated by one NULL code. When only the editor copyright is given, the
            /// photographer copyright part consists of one space followed by a terminating NULL code, then the editor copyright is given. When the field is left
            /// blank, it is treated as unknown.
            /// </remarks>
            internal static readonly MetadataKey Copyright = new(MetadataSection.Image, 33432);

            /// <summary>
            /// Image.ExposureTime
            /// </summary>
            /// <remarks>
            /// Exposure time, given in seconds.
            /// </remarks>
            internal static readonly MetadataKey ExposureTime = new(MetadataSection.Image, 33434);

            /// <summary>
            /// Image.FNumber
            /// </summary>
            /// <remarks>
            /// The F number.
            /// </remarks>
            internal static readonly MetadataKey FNumber = new(MetadataSection.Image, 33437);

            /// <summary>
            /// Image.IPTCNAA
            /// </summary>
            /// <remarks>
            /// Contains an IPTC/NAA record
            /// </remarks>
            internal static readonly MetadataKey IPTCNAA = new(MetadataSection.Image, 33723);

            /// <summary>
            /// Image.ImageResources
            /// </summary>
            /// <remarks>
            /// Contains information embedded by the Adobe Photoshop application
            /// </remarks>
            internal static readonly MetadataKey ImageResources = new(MetadataSection.Image, 34377);

            /// <summary>
            /// Image.ExifTag
            /// </summary>
            /// <remarks>
            /// A pointer to the Exif IFD. Interoperability, Exif IFD has the same structure as that of the IFD specified in TIFF. ordinarily, however, it does not
            /// contain image data as in the case of TIFF.
            /// </remarks>
            internal static readonly MetadataKey ExifTag = new(MetadataSection.Image, 34665);

            /// <summary>
            /// Image.InterColorProfile
            /// </summary>
            /// <remarks>
            /// Contains an InterColor Consortium (ICC) format color space characterization/profile
            /// </remarks>
            internal static readonly MetadataKey InterColorProfile = new(MetadataSection.Image, 34675);

            /// <summary>
            /// Image.ExposureProgram
            /// </summary>
            /// <remarks>
            /// The class of the program used by the camera to set exposure when the picture is taken.
            /// </remarks>
            internal static readonly MetadataKey ExposureProgram = new(MetadataSection.Image, 34850);

            /// <summary>
            /// Image.SpectralSensitivity
            /// </summary>
            /// <remarks>
            /// Indicates the spectral sensitivity of each channel of the camera used.
            /// </remarks>
            internal static readonly MetadataKey SpectralSensitivity = new(MetadataSection.Image, 34852);

            /// <summary>
            /// Image.GPSTag
            /// </summary>
            /// <remarks>
            /// A pointer to the GPS Info IFD. The Interoperability structure of the GPS Info IFD, like that of Exif IFD, has no image data.
            /// </remarks>
            internal static readonly MetadataKey GPSTag = new(MetadataSection.Image, 34853);

            /// <summary>
            /// Image.ISOSpeedRatings
            /// </summary>
            /// <remarks>
            /// Indicates the ISO Speed and ISO Latitude of the camera or input device as specified in ISO 12232.
            /// </remarks>
            internal static readonly MetadataKey ISOSpeedRatings = new(MetadataSection.Image, 34855);

            /// <summary>
            /// Image.OECF
            /// </summary>
            /// <remarks>
            /// Indicates the Opto-Electric Conversion Function (OECF) specified in ISO 14524.
            /// </remarks>
            internal static readonly MetadataKey OECF = new(MetadataSection.Image, 34856);

            /// <summary>
            /// Image.Interlace
            /// </summary>
            /// <remarks>
            /// Indicates the field number of multifield images.
            /// </remarks>
            internal static readonly MetadataKey Interlace = new(MetadataSection.Image, 34857);

            /// <summary>
            /// Image.TimeZoneOffset
            /// </summary>
            /// <remarks>
            /// This optional tag encodes the time zone of the camera clock (relative to Greenwich Mean Time) used to create the DataTimeOriginal tag-value when the
            /// picture was taken. It may also contain the time zone offset of the clock used to create the DateTime tag-value when the image was modified.
            /// </remarks>
            internal static readonly MetadataKey TimeZoneOffset = new(MetadataSection.Image, 34858);

            /// <summary>
            /// Image.SelfTimerMode
            /// </summary>
            /// <remarks>
            /// Number of seconds image capture was delayed from button press.
            /// </remarks>
            internal static readonly MetadataKey SelfTimerMode = new(MetadataSection.Image, 34859);

            /// <summary>
            /// Image.DateTimeOriginal
            /// </summary>
            /// <remarks>
            /// The date and time when the original image data was generated.
            /// </remarks>
            internal static readonly MetadataKey DateTimeOriginal = new(MetadataSection.Image, 36867);

            /// <summary>
            /// Image.CompressedBitsPerPixel
            /// </summary>
            /// <remarks>
            /// Specific to compressed data; states the compressed bits per pixel.
            /// </remarks>
            internal static readonly MetadataKey CompressedBitsPerPixel = new(MetadataSection.Image, 37122);

            /// <summary>
            /// Image.ShutterSpeedValue
            /// </summary>
            /// <remarks>
            /// Shutter speed.
            /// </remarks>
            internal static readonly MetadataKey ShutterSpeedValue = new(MetadataSection.Image, 37377);

            /// <summary>
            /// Image.ApertureValue
            /// </summary>
            /// <remarks>
            /// The lens aperture.
            /// </remarks>
            internal static readonly MetadataKey ApertureValue = new(MetadataSection.Image, 37378);

            /// <summary>
            /// Image.BrightnessValue
            /// </summary>
            /// <remarks>
            /// The value of brightness.
            /// </remarks>
            internal static readonly MetadataKey BrightnessValue = new(MetadataSection.Image, 37379);

            /// <summary>
            /// Image.ExposureBiasValue
            /// </summary>
            /// <remarks>
            /// The exposure bias.
            /// </remarks>
            internal static readonly MetadataKey ExposureBiasValue = new(MetadataSection.Image, 37380);

            /// <summary>
            /// Image.MaxApertureValue
            /// </summary>
            /// <remarks>
            /// The smallest F number of the lens.
            /// </remarks>
            internal static readonly MetadataKey MaxApertureValue = new(MetadataSection.Image, 37381);

            /// <summary>
            /// Image.SubjectDistance
            /// </summary>
            /// <remarks>
            /// The distance to the subject, given in meters.
            /// </remarks>
            internal static readonly MetadataKey SubjectDistance = new(MetadataSection.Image, 37382);

            /// <summary>
            /// Image.MeteringMode
            /// </summary>
            /// <remarks>
            /// The metering mode.
            /// </remarks>
            internal static readonly MetadataKey MeteringMode = new(MetadataSection.Image, 37383);

            /// <summary>
            /// Image.LightSource
            /// </summary>
            /// <remarks>
            /// The kind of light source.
            /// </remarks>
            internal static readonly MetadataKey LightSource = new(MetadataSection.Image, 37384);

            /// <summary>
            /// Image.Flash
            /// </summary>
            /// <remarks>
            /// Indicates the status of flash when the image was shot.
            /// </remarks>
            internal static readonly MetadataKey Flash = new(MetadataSection.Image, 37385);

            /// <summary>
            /// Image.FocalLength
            /// </summary>
            /// <remarks>
            /// The actual focal length of the lens, in mm.
            /// </remarks>
            internal static readonly MetadataKey FocalLength = new(MetadataSection.Image, 37386);

            /// <summary>
            /// Image.FlashEnergy
            /// </summary>
            /// <remarks>
            /// Amount of flash energy (BCPS).
            /// </remarks>
            internal static readonly MetadataKey FlashEnergy = new(MetadataSection.Image, 37387);

            /// <summary>
            /// Image.SpatialFrequencyResponse
            /// </summary>
            /// <remarks>
            /// SFR of the camera.
            /// </remarks>
            internal static readonly MetadataKey SpatialFrequencyResponse = new(MetadataSection.Image, 37388);

            /// <summary>
            /// Image.Noise
            /// </summary>
            /// <remarks>
            /// Noise measurement values.
            /// </remarks>
            internal static readonly MetadataKey Noise = new(MetadataSection.Image, 37389);

            /// <summary>
            /// Image.FocalPlaneXResolution
            /// </summary>
            /// <remarks>
            /// Number of pixels per FocalPlaneResolutionUnit (37392) in ImageWidth direction for main image.
            /// </remarks>
            internal static readonly MetadataKey FocalPlaneXResolution = new(MetadataSection.Image, 37390);

            /// <summary>
            /// Image.FocalPlaneYResolution
            /// </summary>
            /// <remarks>
            /// Number of pixels per FocalPlaneResolutionUnit (37392) in ImageLength direction for main image.
            /// </remarks>
            internal static readonly MetadataKey FocalPlaneYResolution = new(MetadataSection.Image, 37391);

            /// <summary>
            /// Image.FocalPlaneResolutionUnit
            /// </summary>
            /// <remarks>
            /// Unit of measurement for FocalPlaneXResolution(37390) and FocalPlaneYResolution(37391).
            /// </remarks>
            internal static readonly MetadataKey FocalPlaneResolutionUnit = new(MetadataSection.Image, 37392);

            /// <summary>
            /// Image.ImageNumber
            /// </summary>
            /// <remarks>
            /// Number assigned to an image, e.g., in a chained image burst.
            /// </remarks>
            internal static readonly MetadataKey ImageNumber = new(MetadataSection.Image, 37393);

            /// <summary>
            /// Image.SecurityClassification
            /// </summary>
            /// <remarks>
            /// Security classification assigned to the image.
            /// </remarks>
            internal static readonly MetadataKey SecurityClassification = new(MetadataSection.Image, 37394);

            /// <summary>
            /// Image.ImageHistory
            /// </summary>
            /// <remarks>
            /// Record of what has been done to the image.
            /// </remarks>
            internal static readonly MetadataKey ImageHistory = new(MetadataSection.Image, 37395);

            /// <summary>
            /// Image.SubjectLocation
            /// </summary>
            /// <remarks>
            /// Indicates the location and area of the main subject in the overall scene.
            /// </remarks>
            internal static readonly MetadataKey SubjectLocation = new(MetadataSection.Image, 37396);

            /// <summary>
            /// Image.ExposureIndex
            /// </summary>
            /// <remarks>
            /// Encodes the camera exposure index setting when image was captured.
            /// </remarks>
            internal static readonly MetadataKey ExposureIndex = new(MetadataSection.Image, 37397);

            /// <summary>
            /// Image.TIFFEPStandardID
            /// </summary>
            /// <remarks>
            /// Contains four ASCII characters representing the TIFF/EP standard version of a TIFF/EP file, eg '1', '0', '0', '0'
            /// </remarks>
            internal static readonly MetadataKey TIFFEPStandardID = new(MetadataSection.Image, 37398);

            /// <summary>
            /// Image.SensingMethod
            /// </summary>
            /// <remarks>
            /// Type of image sensor.
            /// </remarks>
            internal static readonly MetadataKey SensingMethod = new(MetadataSection.Image, 37399);

            /// <summary>
            /// Image.XPTitle
            /// </summary>
            /// <remarks>
            /// Title tag used by Windows, encoded in UCS2
            /// </remarks>
            internal static readonly MetadataKey XPTitle = new(MetadataSection.Image, 40091);

            /// <summary>
            /// Image.XPComment
            /// </summary>
            /// <remarks>
            /// Comment tag used by Windows, encoded in UCS2
            /// </remarks>
            internal static readonly MetadataKey XPComment = new(MetadataSection.Image, 40092);

            /// <summary>
            /// Image.XPAuthor
            /// </summary>
            /// <remarks>
            /// Author tag used by Windows, encoded in UCS2
            /// </remarks>
            internal static readonly MetadataKey XPAuthor = new(MetadataSection.Image, 40093);

            /// <summary>
            /// Image.XPKeywords
            /// </summary>
            /// <remarks>
            /// Keywords tag used by Windows, encoded in UCS2
            /// </remarks>
            internal static readonly MetadataKey XPKeywords = new(MetadataSection.Image, 40094);

            /// <summary>
            /// Image.XPSubject
            /// </summary>
            /// <remarks>
            /// Subject tag used by Windows, encoded in UCS2
            /// </remarks>
            internal static readonly MetadataKey XPSubject = new(MetadataSection.Image, 40095);

            /// <summary>
            /// Image.PrintImageMatching
            /// </summary>
            /// <remarks>
            /// Print Image Matching, description needed.
            /// </remarks>
            internal static readonly MetadataKey PrintImageMatching = new(MetadataSection.Image, 50341);

            /// <summary>
            /// Image.DNGVersion
            /// </summary>
            /// <remarks>
            /// This tag encodes the DNG four-tier version number. For files compliant with version 1.1.0.0 of the DNG specification, this tag should contain the
            /// bytes: 1, 1, 0, 0.
            /// </remarks>
            internal static readonly MetadataKey DNGVersion = new(MetadataSection.Image, 50706);

            /// <summary>
            /// Image.DNGBackwardVersion
            /// </summary>
            /// <remarks>
            /// This tag specifies the oldest version of the Digital Negative specification for which a file is compatible. Readers shouldnot attempt to read a file
            /// if this tag specifies a version number that is higher than the version number of the specification the reader was based on. In addition to checking
            /// the version tags, readers should, for all tags, check the types, counts, and values, to verify it is able to correctly read the file.
            /// </remarks>
            internal static readonly MetadataKey DNGBackwardVersion = new(MetadataSection.Image, 50707);

            /// <summary>
            /// Image.UniqueCameraModel
            /// </summary>
            /// <remarks>
            /// Defines a unique, non-localized name for the camera model that created the image in the raw file. This name should include the manufacturer's name to
            /// avoid conflicts, and should not be localized, even if the camera name itself is localized for different markets (see LocalizedCameraModel). This
            /// string may be used by reader software to index into per-model preferences and replacement profiles.
            /// </remarks>
            internal static readonly MetadataKey UniqueCameraModel = new(MetadataSection.Image, 50708);

            /// <summary>
            /// Image.LocalizedCameraModel
            /// </summary>
            /// <remarks>
            /// Similar to the UniqueCameraModel field, except the name can be localized for different markets to match the localization of the camera name.
            /// </remarks>
            internal static readonly MetadataKey LocalizedCameraModel = new(MetadataSection.Image, 50709);

            /// <summary>
            /// Image.CFAPlaneColor
            /// </summary>
            /// <remarks>
            /// Provides a mapping between the values in the CFAPattern tag and the plane numbers in LinearRaw space. This is a required tag for non-RGB CFA images.
            /// </remarks>
            internal static readonly MetadataKey CFAPlaneColor = new(MetadataSection.Image, 50710);

            /// <summary>
            /// Image.CFALayout
            /// </summary>
            /// <remarks>
            /// Describes the spatial layout of the CFA.
            /// </remarks>
            internal static readonly MetadataKey CFALayout = new(MetadataSection.Image, 50711);

            /// <summary>
            /// Image.LinearizationTable
            /// </summary>
            /// <remarks>
            /// Describes a lookup table that maps stored values into linear values. This tag is typically used to increase compression ratios by storing the raw data
            /// in a non-linear, more visually uniform space with fewer total encoding levels. If SamplesPerPixel is not equal to one, this single table applies to
            /// all the samples for each pixel.
            /// </remarks>
            internal static readonly MetadataKey LinearizationTable = new(MetadataSection.Image, 50712);

            /// <summary>
            /// Image.BlackLevelRepeatDim
            /// </summary>
            /// <remarks>
            /// Specifies repeat pattern size for the BlackLevel tag.
            /// </remarks>
            internal static readonly MetadataKey BlackLevelRepeatDim = new(MetadataSection.Image, 50713);

            /// <summary>
            /// Image.BlackLevel
            /// </summary>
            /// <remarks>
            /// Specifies the zero light (a.k.a. thermal black or black current) encoding level, as a repeating pattern. The origin of this pattern is the top-left
            /// corner of the ActiveArea rectangle. The values are stored in row-column-sample scan order.
            /// </remarks>
            internal static readonly MetadataKey BlackLevel = new(MetadataSection.Image, 50714);

            /// <summary>
            /// Image.BlackLevelDeltaH
            /// </summary>
            /// <remarks>
            /// If the zero light encoding level is a function of the image column, BlackLevelDeltaH specifies the difference between the zero light encoding level
            /// for each column and the baseline zero light encoding level. If SamplesPerPixel is not equal to one, this single table applies to all the samples for
            /// each pixel.
            /// </remarks>
            internal static readonly MetadataKey BlackLevelDeltaH = new(MetadataSection.Image, 50715);

            /// <summary>
            /// Image.BlackLevelDeltaV
            /// </summary>
            /// <remarks>
            /// If the zero light encoding level is a function of the image row, this tag specifies the difference between the zero light encoding level for each row
            /// and the baseline zero light encoding level. If SamplesPerPixel is not equal to one, this single table applies to all the samples for each pixel.
            /// </remarks>
            internal static readonly MetadataKey BlackLevelDeltaV = new(MetadataSection.Image, 50716);

            /// <summary>
            /// Image.WhiteLevel
            /// </summary>
            /// <remarks>
            /// This tag specifies the fully saturated encoding level for the raw sample values. Saturation is caused either by the sensor itself becoming highly
            /// non-linear in response, or by the camera's analog to digital converter clipping.
            /// </remarks>
            internal static readonly MetadataKey WhiteLevel = new(MetadataSection.Image, 50717);

            /// <summary>
            /// Image.DefaultScale
            /// </summary>
            /// <remarks>
            /// DefaultScale is required for cameras with non-square pixels. It specifies the default scale factors for each direction to convert the image to square
            /// pixels. Typically these factors are selected to approximately preserve total pixel count. For CFA images that use CFALayout equal to 2, 3, 4, or 5,
            /// such as the Fujifilm SuperCCD, these two values should usually differ by a factor of 2.0.
            /// </remarks>
            internal static readonly MetadataKey DefaultScale = new(MetadataSection.Image, 50718);

            /// <summary>
            /// Image.DefaultCropOrigin
            /// </summary>
            /// <remarks>
            /// Raw images often store extra pixels around the edges of the final image. These extra pixels help prevent interpolation artifacts near the edges of the
            /// final image. DefaultCropOrigin specifies the origin of the final image area, in raw image coordinates (i.e., before the DefaultScale has been
            /// applied), relative to the top-left corner of the ActiveArea rectangle.
            /// </remarks>
            internal static readonly MetadataKey DefaultCropOrigin = new(MetadataSection.Image, 50719);

            /// <summary>
            /// Image.DefaultCropSize
            /// </summary>
            /// <remarks>
            /// Raw images often store extra pixels around the edges of the final image. These extra pixels help prevent interpolation artifacts near the edges of the
            /// final image. DefaultCropSize specifies the size of the final image area, in raw image coordinates (i.e., before the DefaultScale has been applied).
            /// </remarks>
            internal static readonly MetadataKey DefaultCropSize = new(MetadataSection.Image, 50720);

            /// <summary>
            /// Image.ColorMatrix1
            /// </summary>
            /// <remarks>
            /// ColorMatrix1 defines a transformation matrix that converts XYZ values to reference camera native color space values, under the first calibration
            /// illuminant. The matrix values are stored in row scan order. The ColorMatrix1 tag is required for all non-monochrome DNG files.
            /// </remarks>
            internal static readonly MetadataKey ColorMatrix1 = new(MetadataSection.Image, 50721);

            /// <summary>
            /// Image.ColorMatrix2
            /// </summary>
            /// <remarks>
            /// ColorMatrix2 defines a transformation matrix that converts XYZ values to reference camera native color space values, under the second calibration
            /// illuminant. The matrix values are stored in row scan order.
            /// </remarks>
            internal static readonly MetadataKey ColorMatrix2 = new(MetadataSection.Image, 50722);

            /// <summary>
            /// Image.CameraCalibration1
            /// </summary>
            /// <remarks>
            /// CameraCalibration1 defines a calibration matrix that transforms reference camera native space values to individual camera native space values under
            /// the first calibration illuminant. The matrix is stored in row scan order. This matrix is stored separately from the matrix specified by the
            /// ColorMatrix1 tag to allow raw converters to swap in replacement color matrices based on UniqueCameraModel tag, while still taking advantage of any
            /// per-individual camera calibration performed by the camera manufacturer.
            /// </remarks>
            internal static readonly MetadataKey CameraCalibration1 = new(MetadataSection.Image, 50723);

            /// <summary>
            /// Image.CameraCalibration2
            /// </summary>
            /// <remarks>
            /// CameraCalibration2 defines a calibration matrix that transforms reference camera native space values to individual camera native space values under
            /// the second calibration illuminant. The matrix is stored in row scan order. This matrix is stored separately from the matrix specified by the
            /// ColorMatrix2 tag to allow raw converters to swap in replacement color matrices based on UniqueCameraModel tag, while still taking advantage of any
            /// per-individual camera calibration performed by the camera manufacturer.
            /// </remarks>
            internal static readonly MetadataKey CameraCalibration2 = new(MetadataSection.Image, 50724);

            /// <summary>
            /// Image.ReductionMatrix1
            /// </summary>
            /// <remarks>
            /// ReductionMatrix1 defines a dimensionality reduction matrix for use as the first stage in converting color camera native space values to XYZ values,
            /// under the first calibration illuminant. This tag may only be used if ColorPlanes is greater than 3. The matrix is stored in row scan order.
            /// </remarks>
            internal static readonly MetadataKey ReductionMatrix1 = new(MetadataSection.Image, 50725);

            /// <summary>
            /// Image.ReductionMatrix2
            /// </summary>
            /// <remarks>
            /// ReductionMatrix2 defines a dimensionality reduction matrix for use as the first stage in converting color camera native space values to XYZ values,
            /// under the second calibration illuminant. This tag may only be used if ColorPlanes is greater than 3. The matrix is stored in row scan order.
            /// </remarks>
            internal static readonly MetadataKey ReductionMatrix2 = new(MetadataSection.Image, 50726);

            /// <summary>
            /// Image.AnalogBalance
            /// </summary>
            /// <remarks>
            /// Normally the stored raw values are not white balanced, since any digital white balancing will reduce the dynamic range of the final image if the user
            /// decides to later adjust the white balance; however, if camera hardware is capable of white balancing the color channels before the signal is
            /// digitized, it can improve the dynamic range of the final image. AnalogBalance defines the gain, either analog (recommended) or digital (not
            /// recommended) that has been applied the stored raw values.
            /// </remarks>
            internal static readonly MetadataKey AnalogBalance = new(MetadataSection.Image, 50727);

            /// <summary>
            /// Image.AsShotNeutral
            /// </summary>
            /// <remarks>
            /// Specifies the selected white balance at time of capture, encoded as the coordinates of a perfectly neutral color in linear reference space values. The
            /// inclusion of this tag precludes the inclusion of the AsShotWhiteXY tag.
            /// </remarks>
            internal static readonly MetadataKey AsShotNeutral = new(MetadataSection.Image, 50728);

            /// <summary>
            /// Image.AsShotWhiteXY
            /// </summary>
            /// <remarks>
            /// Specifies the selected white balance at time of capture, encoded as x-y chromaticity coordinates. The inclusion of this tag precludes the inclusion of
            /// the AsShotNeutral tag.
            /// </remarks>
            internal static readonly MetadataKey AsShotWhiteXY = new(MetadataSection.Image, 50729);

            /// <summary>
            /// Image.BaselineExposure
            /// </summary>
            /// <remarks>
            /// Camera models vary in the trade-off they make between highlight headroom and shadow noise. Some leave a significant amount of highlight headroom
            /// during a normal exposure. This allows significant negative exposure compensation to be applied during raw conversion, but also means normal exposures
            /// will contain more shadow noise. Other models leave less headroom during normal exposures. This allows for less negative exposure compensation, but
            /// results in lower shadow noise for normal exposures. Because of these differences, a raw converter needs to vary the zero point of its exposure
            /// compensation control from model to model. BaselineExposure specifies by how much (in EV units) to move the zero point. Positive values result in
            /// brighter default results, while negative values result in darker default results.
            /// </remarks>
            internal static readonly MetadataKey BaselineExposure = new(MetadataSection.Image, 50730);

            /// <summary>
            /// Image.BaselineNoise
            /// </summary>
            /// <remarks>
            /// Specifies the relative noise level of the camera model at a baseline ISO value of 100, compared to a reference camera model. Since noise levels tend
            /// to vary approximately with the square root of the ISO value, a raw converter can use this value, combined with the current ISO, to estimate the
            /// relative noise level of the current image.
            /// </remarks>
            internal static readonly MetadataKey BaselineNoise = new(MetadataSection.Image, 50731);

            /// <summary>
            /// Image.BaselineSharpness
            /// </summary>
            /// <remarks>
            /// Specifies the relative amount of sharpening required for this camera model, compared to a reference camera model. Camera models vary in the strengths
            /// of their anti-aliasing filters. Cameras with weak or no filters require less sharpening than cameras with strong anti-aliasing filters.
            /// </remarks>
            internal static readonly MetadataKey BaselineSharpness = new(MetadataSection.Image, 50732);

            /// <summary>
            /// Image.BayerGreenSplit
            /// </summary>
            /// <remarks>
            /// Only applies to CFA images using a Bayer pattern filter array. This tag specifies, in arbitrary units, how closely the values of the green pixels in
            /// the blue/green rows track the values of the green pixels in the red/green rows. A value of zero means the two kinds of green pixels track closely,
            /// while a non-zero value means they sometimes diverge. The useful range for this tag is from 0 (no divergence) to about 5000 (quite large divergence).
            /// </remarks>
            internal static readonly MetadataKey BayerGreenSplit = new(MetadataSection.Image, 50733);

            /// <summary>
            /// Image.LinearResponseLimit
            /// </summary>
            /// <remarks>
            /// Some sensors have an unpredictable non-linearity in their response as they near the upper limit of their encoding range. This non-linearity results in
            /// color shifts in the highlight areas of the resulting image unless the raw converter compensates for this effect. LinearResponseLimit specifies the
            /// fraction of the encoding range above which the response may become significantly non-linear.
            /// </remarks>
            internal static readonly MetadataKey LinearResponseLimit = new(MetadataSection.Image, 50734);

            /// <summary>
            /// Image.CameraSerialNumber
            /// </summary>
            /// <remarks>
            /// CameraSerialNumber contains the serial number of the camera or camera body that captured the image.
            /// </remarks>
            internal static readonly MetadataKey CameraSerialNumber = new(MetadataSection.Image, 50735);

            /// <summary>
            /// Image.LensInfo
            /// </summary>
            /// <remarks>
            /// Contains information about the lens that captured the image. If the minimum f-stops are unknown, they should be encoded as 0/0.
            /// </remarks>
            internal static readonly MetadataKey LensInfo = new(MetadataSection.Image, 50736);

            /// <summary>
            /// Image.ChromaBlurRadius
            /// </summary>
            /// <remarks>
            /// ChromaBlurRadius provides a hint to the DNG reader about how much chroma blur should be applied to the image. If this tag is omitted, the reader will
            /// use its default amount of chroma blurring. Normally this tag is only included for non-CFA images, since the amount of chroma blur required for mosaic
            /// images is highly dependent on the de-mosaic algorithm, in which case the DNG reader's default value is likely optimized for its particular de-mosaic
            /// algorithm.
            /// </remarks>
            internal static readonly MetadataKey ChromaBlurRadius = new(MetadataSection.Image, 50737);

            /// <summary>
            /// Image.AntiAliasStrength
            /// </summary>
            /// <remarks>
            /// Provides a hint to the DNG reader about how strong the camera's anti-alias filter is. A value of 0.0 means no anti-alias filter (i.e., the camera is
            /// prone to aliasing artifacts with some subjects), while a value of 1.0 means a strong anti-alias filter (i.e., the camera almost never has aliasing
            /// artifacts).
            /// </remarks>
            internal static readonly MetadataKey AntiAliasStrength = new(MetadataSection.Image, 50738);

            /// <summary>
            /// Image.ShadowScale
            /// </summary>
            /// <remarks>
            /// This tag is used by Adobe Camera Raw to control the sensitivity of its 'Shadows' slider.
            /// </remarks>
            internal static readonly MetadataKey ShadowScale = new(MetadataSection.Image, 50739);

            /// <summary>
            /// Image.DNGPrivateData
            /// </summary>
            /// <remarks>
            /// Provides a way for camera manufacturers to store private data in the DNG file for use by their own raw converters, and to have that data preserved by
            /// programs that edit DNG files.
            /// </remarks>
            internal static readonly MetadataKey DNGPrivateData = new(MetadataSection.Image, 50740);

            /// <summary>
            /// Image.MakerNoteSafety
            /// </summary>
            /// <remarks>
            /// MakerNoteSafety lets the DNG reader know whether the EXIF MakerNote tag is safe to preserve along with the rest of the EXIF data. File browsers and
            /// other image management software processing an image with a preserved MakerNote should be aware that any thumbnail image embedded in the MakerNote may
            /// be stale, and may not reflect the current state of the full size image.
            /// </remarks>
            internal static readonly MetadataKey MakerNoteSafety = new(MetadataSection.Image, 50741);

            /// <summary>
            /// Image.CalibrationIlluminant1
            /// </summary>
            /// <remarks>
            /// The illuminant used for the first set of color calibration tags (ColorMatrix1, CameraCalibration1, ReductionMatrix1). The legal values for this tag
            /// are the same as the legal values for the LightSource EXIF tag.
            /// </remarks>
            internal static readonly MetadataKey CalibrationIlluminant1 = new(MetadataSection.Image, 50778);

            /// <summary>
            /// Image.CalibrationIlluminant2
            /// </summary>
            /// <remarks>
            /// The illuminant used for an optional second set of color calibration tags (ColorMatrix2, CameraCalibration2, ReductionMatrix2). The legal values for
            /// this tag are the same as the legal values for the CalibrationIlluminant1 tag; however, if both are included, neither is allowed to have a value of 0
            /// (unknown).
            /// </remarks>
            internal static readonly MetadataKey CalibrationIlluminant2 = new(MetadataSection.Image, 50779);

            /// <summary>
            /// Image.BestQualityScale
            /// </summary>
            /// <remarks>
            /// For some cameras, the best possible image quality is not achieved by preserving the total pixel count during conversion. For example, Fujifilm
            /// SuperCCD images have maximum detail when their total pixel count is doubled. This tag specifies the amount by which the values of the DefaultScale tag
            /// need to be multiplied to achieve the best quality image size.
            /// </remarks>
            internal static readonly MetadataKey BestQualityScale = new(MetadataSection.Image, 50780);

            /// <summary>
            /// Image.RawDataUniqueID
            /// </summary>
            /// <remarks>
            /// This tag contains a 16-byte unique identifier for the raw image data in the DNG file. DNG readers can use this tag to recognize a particular raw
            /// image, even if the file's name or the metadata contained in the file has been changed. If a DNG writer creates such an identifier, it should do so
            /// using an algorithm that will ensure that it is very unlikely two different images will end up having the same identifier.
            /// </remarks>
            internal static readonly MetadataKey RawDataUniqueID = new(MetadataSection.Image, 50781);

            /// <summary>
            /// Image.OriginalRawFileName
            /// </summary>
            /// <remarks>
            /// If the DNG file was converted from a non-DNG raw file, then this tag contains the file name of that original raw file.
            /// </remarks>
            internal static readonly MetadataKey OriginalRawFileName = new(MetadataSection.Image, 50827);

            /// <summary>
            /// Image.OriginalRawFileData
            /// </summary>
            /// <remarks>
            /// If the DNG file was converted from a non-DNG raw file, then this tag contains the compressed contents of that original raw file. The contents of this
            /// tag always use the big-endian byte order. The tag contains a sequence of data blocks. Future versions of the DNG specification may define additional
            /// data blocks, so DNG readers should ignore extra bytes when parsing this tag. DNG readers should also detect the case where data blocks are missing
            /// from the end of the sequence, and should assume a default value for all the missing blocks. There are no padding or alignment bytes between data
            /// blocks.
            /// </remarks>
            internal static readonly MetadataKey OriginalRawFileData = new(MetadataSection.Image, 50828);

            /// <summary>
            /// Image.ActiveArea
            /// </summary>
            /// <remarks>
            /// This rectangle defines the active (non-masked) pixels of the sensor. The order of the rectangle coordinates is: top, left, bottom, right.
            /// </remarks>
            internal static readonly MetadataKey ActiveArea = new(MetadataSection.Image, 50829);

            /// <summary>
            /// Image.MaskedAreas
            /// </summary>
            /// <remarks>
            /// This tag contains a list of non-overlapping rectangle coordinates of fully masked pixels, which can be optionally used by DNG readers to measure the
            /// black encoding level. The order of each rectangle's coordinates is: top, left, bottom, right. If the raw image data has already had its black encoding
            /// level subtracted, then this tag should not be used, since the masked pixels are no longer useful.
            /// </remarks>
            internal static readonly MetadataKey MaskedAreas = new(MetadataSection.Image, 50830);

            /// <summary>
            /// Image.AsShotICCProfile
            /// </summary>
            /// <remarks>
            /// This tag contains an ICC profile that, in conjunction with the AsShotPreProfileMatrix tag, provides the camera manufacturer with a way to specify a
            /// default color rendering from camera color space coordinates (linear reference values) into the ICC profile connection space. The ICC profile
            /// connection space is an output referred colorimetric space, whereas the other color calibration tags in DNG specify a conversion into a scene referred
            /// colorimetric space. This means that the rendering in this profile should include any desired tone and gamut mapping needed to convert between scene
            /// referred values and output referred values.
            /// </remarks>
            internal static readonly MetadataKey AsShotICCProfile = new(MetadataSection.Image, 50831);

            /// <summary>
            /// Image.AsShotPreProfileMatrix
            /// </summary>
            /// <remarks>
            /// This tag is used in conjunction with the AsShotICCProfile tag. It specifies a matrix that should be applied to the camera color space coordinates
            /// before processing the values through the ICC profile specified in the AsShotICCProfile tag. The matrix is stored in the row scan order. If ColorPlanes
            /// is greater than three, then this matrix can (but is not required to) reduce the dimensionality of the color data down to three components, in which
            /// case the AsShotICCProfile should have three rather than ColorPlanes input components.
            /// </remarks>
            internal static readonly MetadataKey AsShotPreProfileMatrix = new(MetadataSection.Image, 50832);

            /// <summary>
            /// Image.CurrentICCProfile
            /// </summary>
            /// <remarks>
            /// This tag is used in conjunction with the CurrentPreProfileMatrix tag. The CurrentICCProfile and CurrentPreProfileMatrix tags have the same purpose and
            /// usage as the AsShotICCProfile and AsShotPreProfileMatrix tag pair, except they are for use by raw file editors rather than camera manufacturers.
            /// </remarks>
            internal static readonly MetadataKey CurrentICCProfile = new(MetadataSection.Image, 50833);

            /// <summary>
            /// Image.CurrentPreProfileMatrix
            /// </summary>
            /// <remarks>
            /// This tag is used in conjunction with the CurrentICCProfile tag. The CurrentICCProfile and CurrentPreProfileMatrix tags have the same purpose and usage
            /// as the AsShotICCProfile and AsShotPreProfileMatrix tag pair, except they are for use by raw file editors rather than camera manufacturers.
            /// </remarks>
            internal static readonly MetadataKey CurrentPreProfileMatrix = new(MetadataSection.Image, 50834);

            /// <summary>
            /// Image.ColorimetricReference
            /// </summary>
            /// <remarks>
            /// The DNG color model documents a transform between camera colors and CIE XYZ values. This tag describes the colorimetric reference for the CIE XYZ
            /// values. 0 = The XYZ values are scene-referred. 1 = The XYZ values are output-referred, using the ICC profile perceptual dynamic range. This tag allows
            /// output-referred data to be stored in DNG files and still processed correctly by DNG readers.
            /// </remarks>
            internal static readonly MetadataKey ColorimetricReference = new(MetadataSection.Image, 50879);

            /// <summary>
            /// Image.CameraCalibrationSignature
            /// </summary>
            /// <remarks>
            /// A UTF-8 encoded string associated with the CameraCalibration1 and CameraCalibration2 tags. The CameraCalibration1 and CameraCalibration2 tags should
            /// only be used in the DNG color transform if the string stored in the CameraCalibrationSignature tag exactly matches the string stored in the
            /// ProfileCalibrationSignature tag for the selected camera profile.
            /// </remarks>
            internal static readonly MetadataKey CameraCalibrationSignature = new(MetadataSection.Image, 50931);

            /// <summary>
            /// Image.ProfileCalibrationSignature
            /// </summary>
            /// <remarks>
            /// A UTF-8 encoded string associated with the camera profile tags. The CameraCalibration1 and CameraCalibration2 tags should only be used in the DNG
            /// color transfer if the string stored in the CameraCalibrationSignature tag exactly matches the string stored in the ProfileCalibrationSignature tag for
            /// the selected camera profile.
            /// </remarks>
            internal static readonly MetadataKey ProfileCalibrationSignature = new(MetadataSection.Image, 50932);

            /// <summary>
            /// Image.AsShotProfileName
            /// </summary>
            /// <remarks>
            /// A UTF-8 encoded string containing the name of the "as shot" camera profile, if any.
            /// </remarks>
            internal static readonly MetadataKey AsShotProfileName = new(MetadataSection.Image, 50934);

            /// <summary>
            /// Image.NoiseReductionApplied
            /// </summary>
            /// <remarks>
            /// This tag indicates how much noise reduction has been applied to the raw data on a scale of 0.0 to 1.0. A 0.0 value indicates that no noise reduction
            /// has been applied. A 1.0 value indicates that the "ideal" amount of noise reduction has been applied, i.e. that the DNG reader should not apply
            /// additional noise reduction by default. A value of 0/0 indicates that this parameter is unknown.
            /// </remarks>
            internal static readonly MetadataKey NoiseReductionApplied = new(MetadataSection.Image, 50935);

            /// <summary>
            /// Image.ProfileName
            /// </summary>
            /// <remarks>
            /// A UTF-8 encoded string containing the name of the camera profile. This tag is optional if there is only a single camera profile stored in the file but
            /// is required for all camera profiles if there is more than one camera profile stored in the file.
            /// </remarks>
            internal static readonly MetadataKey ProfileName = new(MetadataSection.Image, 50936);

            /// <summary>
            /// Image.ProfileHueSatMapDims
            /// </summary>
            /// <remarks>
            /// This tag specifies the number of input samples in each dimension of the hue/saturation/value mapping tables. The data for these tables are stored in
            /// ProfileHueSatMapData1 and ProfileHueSatMapData2 tags. The most common case has ValueDivisions equal to 1, so only hue and saturation are used as
            /// inputs to the mapping table.
            /// </remarks>
            internal static readonly MetadataKey ProfileHueSatMapDims = new(MetadataSection.Image, 50937);

            /// <summary>
            /// Image.ProfileHueSatMapData1
            /// </summary>
            /// <remarks>
            /// This tag contains the data for the first hue/saturation/value mapping table. Each entry of the table contains three 32-bit IEEE floating-point values.
            /// The first entry is hue shift in degrees; the second entry is saturation scale factor; and the third entry is a value scale factor. The table entries
            /// are stored in the tag in nested loop order, with the value divisions in the outer loop, the hue divisions in the middle loop, and the saturation
            /// divisions in the inner loop. All zero input saturation entries are required to have a value scale factor of 1.0.
            /// </remarks>
            internal static readonly MetadataKey ProfileHueSatMapData1 = new(MetadataSection.Image, 50938);

            /// <summary>
            /// Image.ProfileHueSatMapData2
            /// </summary>
            /// <remarks>
            /// This tag contains the data for the second hue/saturation/value mapping table. Each entry of the table contains three 32-bit IEEE floating-point
            /// values. The first entry is hue shift in degrees; the second entry is a saturation scale factor; and the third entry is a value scale factor. The table
            /// entries are stored in the tag in nested loop order, with the value divisions in the outer loop, the hue divisions in the middle loop, and the
            /// saturation divisions in the inner loop. All zero input saturation entries are required to have a value scale factor of 1.0.
            /// </remarks>
            internal static readonly MetadataKey ProfileHueSatMapData2 = new(MetadataSection.Image, 50939);

            /// <summary>
            /// Image.ProfileToneCurve
            /// </summary>
            /// <remarks>
            /// This tag contains a default tone curve that can be applied while processing the image as a starting point for user adjustments. The curve is specified
            /// as a list of 32-bit IEEE floating-point value pairs in linear gamma. Each sample has an input value in the range of 0.0 to 1.0, and an output value in
            /// the range of 0.0 to 1.0. The first sample is required to be (0.0, 0.0), and the last sample is required to be (1.0, 1.0). Interpolated the curve using
            /// a cubic spline.
            /// </remarks>
            internal static readonly MetadataKey ProfileToneCurve = new(MetadataSection.Image, 50940);

            /// <summary>
            /// Image.ProfileEmbedPolicy
            /// </summary>
            /// <remarks>
            /// This tag contains information about the usage rules for the associated camera profile.
            /// </remarks>
            internal static readonly MetadataKey ProfileEmbedPolicy = new(MetadataSection.Image, 50941);

            /// <summary>
            /// Image.ProfileCopyright
            /// </summary>
            /// <remarks>
            /// A UTF-8 encoded string containing the copyright information for the camera profile. This string always should be preserved along with the other camera
            /// profile tags.
            /// </remarks>
            internal static readonly MetadataKey ProfileCopyright = new(MetadataSection.Image, 50942);

            /// <summary>
            /// Image.ForwardMatrix1
            /// </summary>
            /// <remarks>
            /// This tag defines a matrix that maps white balanced camera colors to XYZ D50 colors.
            /// </remarks>
            internal static readonly MetadataKey ForwardMatrix1 = new(MetadataSection.Image, 50964);

            /// <summary>
            /// Image.ForwardMatrix2
            /// </summary>
            /// <remarks>
            /// This tag defines a matrix that maps white balanced camera colors to XYZ D50 colors.
            /// </remarks>
            internal static readonly MetadataKey ForwardMatrix2 = new(MetadataSection.Image, 50965);

            /// <summary>
            /// Image.PreviewApplicationName
            /// </summary>
            /// <remarks>
            /// A UTF-8 encoded string containing the name of the application that created the preview stored in the IFD.
            /// </remarks>
            internal static readonly MetadataKey PreviewApplicationName = new(MetadataSection.Image, 50966);

            /// <summary>
            /// Image.PreviewApplicationVersion
            /// </summary>
            /// <remarks>
            /// A UTF-8 encoded string containing the version number of the application that created the preview stored in the IFD.
            /// </remarks>
            internal static readonly MetadataKey PreviewApplicationVersion = new(MetadataSection.Image, 50967);

            /// <summary>
            /// Image.PreviewSettingsName
            /// </summary>
            /// <remarks>
            /// A UTF-8 encoded string containing the name of the conversion settings (for example, snapshot name) used for the preview stored in the IFD.
            /// </remarks>
            internal static readonly MetadataKey PreviewSettingsName = new(MetadataSection.Image, 50968);

            /// <summary>
            /// Image.PreviewSettingsDigest
            /// </summary>
            /// <remarks>
            /// A unique ID of the conversion settings (for example, MD5 digest) used to render the preview stored in the IFD.
            /// </remarks>
            internal static readonly MetadataKey PreviewSettingsDigest = new(MetadataSection.Image, 50969);

            /// <summary>
            /// Image.PreviewColorSpace
            /// </summary>
            /// <remarks>
            /// This tag specifies the color space in which the rendered preview in this IFD is stored. The default value for this tag is sRGB for color previews and
            /// Gray Gamma 2.2 for monochrome previews.
            /// </remarks>
            internal static readonly MetadataKey PreviewColorSpace = new(MetadataSection.Image, 50970);

            /// <summary>
            /// Image.PreviewDateTime
            /// </summary>
            /// <remarks>
            /// This tag is an ASCII string containing the name of the date/time at which the preview stored in the IFD was rendered. The date/time is encoded using
            /// ISO 8601 format.
            /// </remarks>
            internal static readonly MetadataKey PreviewDateTime = new(MetadataSection.Image, 50971);

            /// <summary>
            /// Image.RawImageDigest
            /// </summary>
            /// <remarks>
            /// This tag is an MD5 digest of the raw image data. All pixels in the image are processed in row-scan order. Each pixel is zero padded to 16 or 32 bits
            /// deep (16-bit for data less than or equal to 16 bits deep, 32-bit otherwise). The data for each pixel is processed in little-endian byte order.
            /// </remarks>
            internal static readonly MetadataKey RawImageDigest = new(MetadataSection.Image, 50972);

            /// <summary>
            /// Image.OriginalRawFileDigest
            /// </summary>
            /// <remarks>
            /// This tag is an MD5 digest of the data stored in the OriginalRawFileData tag.
            /// </remarks>
            internal static readonly MetadataKey OriginalRawFileDigest = new(MetadataSection.Image, 50973);

            /// <summary>
            /// Image.SubTileBlockSize
            /// </summary>
            /// <remarks>
            /// Normally, the pixels within a tile are stored in simple row-scan order. This tag specifies that the pixels within a tile should be grouped first into
            /// rectangular blocks of the specified size. These blocks are stored in row-scan order. Within each block, the pixels are stored in row-scan order. The
            /// use of a non-default value for this tag requires setting the DNGBackwardVersion tag to at least 1.2.0.0.
            /// </remarks>
            internal static readonly MetadataKey SubTileBlockSize = new(MetadataSection.Image, 50974);

            /// <summary>
            /// Image.RowInterleaveFactor
            /// </summary>
            /// <remarks>
            /// This tag specifies that rows of the image are stored in interleaved order. The value of the tag specifies the number of interleaved fields. The use of
            /// a non-default value for this tag requires setting the DNGBackwardVersion tag to at least 1.2.0.0.
            /// </remarks>
            internal static readonly MetadataKey RowInterleaveFactor = new(MetadataSection.Image, 50975);

            /// <summary>
            /// Image.ProfileLookTableDims
            /// </summary>
            /// <remarks>
            /// This tag specifies the number of input samples in each dimension of a default "look" table. The data for this table is stored in the
            /// ProfileLookTableData tag.
            /// </remarks>
            internal static readonly MetadataKey ProfileLookTableDims = new(MetadataSection.Image, 50981);

            /// <summary>
            /// Image.ProfileLookTableData
            /// </summary>
            /// <remarks>
            /// This tag contains a default "look" table that can be applied while processing the image as a starting point for user adjustment. This table uses the
            /// same format as the tables stored in the ProfileHueSatMapData1 and ProfileHueSatMapData2 tags, and is applied in the same color space. However, it
            /// should be applied later in the processing pipe, after any exposure compensation and/or fill light stages, but before any tone curve stage. Each entry
            /// of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees, the second entry is a saturation scale factor,
            /// and the third entry is a value scale factor. The table entries are stored in the tag in nested loop order, with the value divisions in the outer loop,
            /// the hue divisions in the middle loop, and the saturation divisions in the inner loop. All zero input saturation entries are required to have a value
            /// scale factor of 1.0.
            /// </remarks>
            internal static readonly MetadataKey ProfileLookTableData = new(MetadataSection.Image, 50982);

            /// <summary>
            /// Image.OpcodeList1
            /// </summary>
            /// <remarks>
            /// Specifies the list of opcodes that should be applied to the raw image, as read directly from the file.
            /// </remarks>
            internal static readonly MetadataKey OpcodeList1 = new(MetadataSection.Image, 51008);

            /// <summary>
            /// Image.OpcodeList2
            /// </summary>
            /// <remarks>
            /// Specifies the list of opcodes that should be applied to the raw image, just after it has been mapped to linear reference values.
            /// </remarks>
            internal static readonly MetadataKey OpcodeList2 = new(MetadataSection.Image, 51009);

            /// <summary>
            /// Image.OpcodeList3
            /// </summary>
            /// <remarks>
            /// Specifies the list of opcodes that should be applied to the raw image, just after it has been demosaiced.
            /// </remarks>
            internal static readonly MetadataKey OpcodeList3 = new(MetadataSection.Image, 51022);

            /// <summary>
            /// Image.NoiseProfile
            /// </summary>
            /// <remarks>
            /// NoiseProfile describes the amount of noise in a raw image. Specifically, this tag models the amount of signal-dependent photon (shot) noise and
            /// signal-independent sensor readout noise, two common sources of noise in raw images. The model assumes that the noise is white and spatially
            /// independent, ignoring fixed pattern effects and other sources of noise (e.g., pixel response non-uniformity, spatially-dependent thermal effects,
            /// etc.).
            /// </remarks>
            internal static readonly MetadataKey NoiseProfile = new(MetadataSection.Image, 51041);

            /// <summary>
            /// Image.TimeCodes
            /// </summary>
            /// <remarks>
            /// The optional TimeCodes tag shall contain an ordered array of time codes. All time codes shall be 8 bytes long and in binary format. The tag may
            /// contain from 1 to 10 time codes. When the tag contains more than one time code, the first one shall be the default time code. This specification does
            /// not prescribe how to use multiple time codes. Each time code shall be as defined for the 8-byte time code structure in SMPTE 331M-2004, Section 8.3.
            /// See also SMPTE 12-1-2008 and SMPTE 309-1999.
            /// </remarks>
            internal static readonly MetadataKey TimeCodes = new(MetadataSection.Image, 51043);

            /// <summary>
            /// Image.FrameRate
            /// </summary>
            /// <remarks>
            /// The optional FrameRate tag shall specify the video frame rate in number of image frames per second, expressed as a signed rational number. The
            /// numerator shall be non-negative and the denominator shall be positive. This field value is identical to the sample rate field in SMPTE 377-1-2009.
            /// </remarks>
            internal static readonly MetadataKey FrameRate = new(MetadataSection.Image, 51044);

            /// <summary>
            /// Image.TStop
            /// </summary>
            /// <remarks>
            /// The optional TStop tag shall specify the T-stop of the actual lens, expressed as an unsigned rational number. T-stop is also known as T-number or the
            /// photometric aperture of the lens. (F-number is the geometric aperture of the lens.) When the exact value is known, the T-stop shall be specified using
            /// a single number. Alternately, two numbers shall be used to indicate a T-stop range, in which case the first number shall be the minimum T-stop and the
            /// second number shall be the maximum T-stop.
            /// </remarks>
            internal static readonly MetadataKey TStop = new(MetadataSection.Image, 51058);

            /// <summary>
            /// Image.ReelName
            /// </summary>
            /// <remarks>
            /// The optional ReelName tag shall specify a name for a sequence of images, where each image in the sequence has a unique image identifier (including but
            /// not limited to file name, frame number, date time, time code).
            /// </remarks>
            internal static readonly MetadataKey ReelName = new(MetadataSection.Image, 51081);

            /// <summary>
            /// Image.CameraLabel
            /// </summary>
            /// <remarks>
            /// The optional CameraLabel tag shall specify a text label for how the camera is used or assigned in this clip. This tag is similar to CameraLabel in
            /// XMP.
            /// </remarks>
            internal static readonly MetadataKey CameraLabel = new(MetadataSection.Image, 51105);

        }

        internal static class Exif
        {
            /// <summary>
            /// Exif.ExposureTime
            /// </summary>
            /// <remarks>
            /// Exposure time, given in seconds (sec).
            /// </remarks>
            internal static readonly MetadataKey ExposureTime = new(MetadataSection.Exif, 33434);

            /// <summary>
            /// Exif.FNumber
            /// </summary>
            /// <remarks>
            /// The F number.
            /// </remarks>
            internal static readonly MetadataKey FNumber = new(MetadataSection.Exif, 33437);

            /// <summary>
            /// Exif.ExposureProgram
            /// </summary>
            /// <remarks>
            /// The class of the program used by the camera to set exposure when the picture is taken.
            /// </remarks>
            internal static readonly MetadataKey ExposureProgram = new(MetadataSection.Exif, 34850);

            /// <summary>
            /// Exif.SpectralSensitivity
            /// </summary>
            /// <remarks>
            /// Indicates the spectral sensitivity of each channel of the camera used. The tag value is an ASCII string compatible with the standard developed by the
            /// ASTM Technical Committee.
            /// </remarks>
            internal static readonly MetadataKey SpectralSensitivity = new(MetadataSection.Exif, 34852);

            /// <summary>
            /// Exif.ISOSpeedRatings
            /// </summary>
            /// <remarks>
            /// Indicates the ISO Speed and ISO Latitude of the camera or input device as specified in ISO 12232.
            /// </remarks>
            internal static readonly MetadataKey ISOSpeedRatings = new(MetadataSection.Exif, 34855);

            /// <summary>
            /// Exif.OECF
            /// </summary>
            /// <remarks>
            /// Indicates the Opto-Electoric Conversion Function (OECF) specified in ISO 14524. 'OECF' is the relationship between the camera optical input and the
            /// image values.
            /// </remarks>
            internal static readonly MetadataKey OECF = new(MetadataSection.Exif, 34856);

            /// <summary>
            /// Exif.SensitivityType
            /// </summary>
            /// <remarks>
            /// The SensitivityType tag indicates which one of the parameters of ISO12232 is the PhotographicSensitivity tag. Although it is an optional tag, it
            /// should be recorded when a PhotographicSensitivity tag is recorded. Value = 4, 5, 6, or 7 may be used in case that the values of plural parameters are
            /// the same.
            /// </remarks>
            internal static readonly MetadataKey SensitivityType = new(MetadataSection.Exif, 34864);

            /// <summary>
            /// Exif.StandardOutputSensitivity
            /// </summary>
            /// <remarks>
            /// This tag indicates the standard output sensitivity value of a camera or input device defined in ISO 12232. When recording this tag, the
            /// PhotographicSensitivity and SensitivityType tags shall also be recorded.
            /// </remarks>
            internal static readonly MetadataKey StandardOutputSensitivity = new(MetadataSection.Exif, 34865);

            /// <summary>
            /// Exif.RecommendedExposureIndex
            /// </summary>
            /// <remarks>
            /// This tag indicates the recommended exposure index value of a camera or input device defined in ISO 12232. When recording this tag, the
            /// PhotographicSensitivity and SensitivityType tags shall also be recorded.
            /// </remarks>
            internal static readonly MetadataKey RecommendedExposureIndex = new(MetadataSection.Exif, 34866);

            /// <summary>
            /// Exif.ISOSpeed
            /// </summary>
            /// <remarks>
            /// This tag indicates the ISO speed value of a camera or input device that is defined in ISO 12232. When recording this tag, the PhotographicSensitivity
            /// and SensitivityType tags shall also be recorded.
            /// </remarks>
            internal static readonly MetadataKey ISOSpeed = new(MetadataSection.Exif, 34867);

            /// <summary>
            /// Exif.ISOSpeedLatitudeyyy
            /// </summary>
            /// <remarks>
            /// This tag indicates the ISO speed latitude yyy value of a camera or input device that is defined in ISO 12232. However, this tag shall not be recorded
            /// without ISOSpeed and ISOSpeedLatitudezzz.
            /// </remarks>
            internal static readonly MetadataKey ISOSpeedLatitudeyyy = new(MetadataSection.Exif, 34868);

            /// <summary>
            /// Exif.ISOSpeedLatitudezzz
            /// </summary>
            /// <remarks>
            /// This tag indicates the ISO speed latitude zzz value of a camera or input device that is defined in ISO 12232. However, this tag shall not be recorded
            /// without ISOSpeed and ISOSpeedLatitudeyyy.
            /// </remarks>
            internal static readonly MetadataKey ISOSpeedLatitudezzz = new(MetadataSection.Exif, 34869);

            /// <summary>
            /// Exif.ExifVersion
            /// </summary>
            /// <remarks>
            /// The version of this standard supported. Nonexistence of this field is taken to mean nonconformance to the standard.
            /// </remarks>
            internal static readonly MetadataKey ExifVersion = new(MetadataSection.Exif, 36864);

            /// <summary>
            /// Exif.DateTimeOriginal
            /// </summary>
            /// <remarks>
            /// The date and time when the original image data was generated. For a digital still camera the date and time the picture was taken are recorded.
            /// </remarks>
            internal static readonly MetadataKey DateTimeOriginal = new(MetadataSection.Exif, 36867);

            /// <summary>
            /// Exif.DateTimeDigitized
            /// </summary>
            /// <remarks>
            /// The date and time when the image was stored as digital data.
            /// </remarks>
            internal static readonly MetadataKey DateTimeDigitized = new(MetadataSection.Exif, 36868);

            /// <summary>
            /// Exif.ComponentsConfiguration
            /// </summary>
            /// <remarks>
            /// Information specific to compressed data. The channels of each component are arranged in order from the 1st component to the 4th. For uncompressed data
            /// the data arrangement is given in the 'PhotometricInterpretation' tag. However, since 'PhotometricInterpretation' can only express the order of Y, Cb
            /// and Cr, this tag is provided for cases when compressed data uses components other than Y, Cb, and Cr and to enable support of other sequences.
            /// </remarks>
            internal static readonly MetadataKey ComponentsConfiguration = new(MetadataSection.Exif, 37121);

            /// <summary>
            /// Exif.CompressedBitsPerPixel
            /// </summary>
            /// <remarks>
            /// Information specific to compressed data. The compression mode used for a compressed image is indicated in unit bits per pixel.
            /// </remarks>
            internal static readonly MetadataKey CompressedBitsPerPixel = new(MetadataSection.Exif, 37122);

            /// <summary>
            /// Exif.ShutterSpeedValue
            /// </summary>
            /// <remarks>
            /// Shutter speed. The unit is the APEX (Additive System of Photographic Exposure) setting.
            /// </remarks>
            internal static readonly MetadataKey ShutterSpeedValue = new(MetadataSection.Exif, 37377);

            /// <summary>
            /// Exif.ApertureValue
            /// </summary>
            /// <remarks>
            /// The lens aperture. The unit is the APEX value.
            /// </remarks>
            internal static readonly MetadataKey ApertureValue = new(MetadataSection.Exif, 37378);

            /// <summary>
            /// Exif.BrightnessValue
            /// </summary>
            /// <remarks>
            /// The value of brightness. The unit is the APEX value. Ordinarily it is given in the range of -99.99 to 99.99.
            /// </remarks>
            internal static readonly MetadataKey BrightnessValue = new(MetadataSection.Exif, 37379);

            /// <summary>
            /// Exif.ExposureBiasValue
            /// </summary>
            /// <remarks>
            /// The exposure bias. The units is the APEX value. Ordinarily it is given in the range of -99.99 to 99.99.
            /// </remarks>
            internal static readonly MetadataKey ExposureBiasValue = new(MetadataSection.Exif, 37380);

            /// <summary>
            /// Exif.MaxApertureValue
            /// </summary>
            /// <remarks>
            /// The smallest F number of the lens. The unit is the APEX value. Ordinarily it is given in the range of 00.00 to 99.99, but it is not limited to this
            /// range.
            /// </remarks>
            internal static readonly MetadataKey MaxApertureValue = new(MetadataSection.Exif, 37381);

            /// <summary>
            /// Exif.SubjectDistance
            /// </summary>
            /// <remarks>
            /// The distance to the subject, given in meters.
            /// </remarks>
            internal static readonly MetadataKey SubjectDistance = new(MetadataSection.Exif, 37382);

            /// <summary>
            /// Exif.MeteringMode
            /// </summary>
            /// <remarks>
            /// The metering mode.
            /// </remarks>
            internal static readonly MetadataKey MeteringMode = new(MetadataSection.Exif, 37383);

            /// <summary>
            /// Exif.LightSource
            /// </summary>
            /// <remarks>
            /// The kind of light source.
            /// </remarks>
            internal static readonly MetadataKey LightSource = new(MetadataSection.Exif, 37384);

            /// <summary>
            /// Exif.Flash
            /// </summary>
            /// <remarks>
            /// This tag is recorded when an image is taken using a strobe light (flash).
            /// </remarks>
            internal static readonly MetadataKey Flash = new(MetadataSection.Exif, 37385);

            /// <summary>
            /// Exif.FocalLength
            /// </summary>
            /// <remarks>
            /// The actual focal length of the lens, in mm. Conversion is not made to the focal length of a 35 mm film camera.
            /// </remarks>
            internal static readonly MetadataKey FocalLength = new(MetadataSection.Exif, 37386);

            /// <summary>
            /// Exif.SubjectArea
            /// </summary>
            /// <remarks>
            /// This tag indicates the location and area of the main subject in the overall scene.
            /// </remarks>
            internal static readonly MetadataKey SubjectArea = new(MetadataSection.Exif, 37396);

            /// <summary>
            /// Exif.MakerNote
            /// </summary>
            /// <remarks>
            /// A tag for manufacturers of Exif writers to record any desired information. The contents are up to the manufacturer.
            /// </remarks>
            internal static readonly MetadataKey MakerNote = new(MetadataSection.Exif, 37500);

            /// <summary>
            /// Exif.UserComment
            /// </summary>
            /// <remarks>
            /// A tag for Exif users to write keywords or comments on the image besides those in 'ImageDescription', and without the character code limitations of the
            /// 'ImageDescription' tag.
            /// </remarks>
            internal static readonly MetadataKey UserComment = new(MetadataSection.Exif, 37510);

            /// <summary>
            /// Exif.SubSecTime
            /// </summary>
            /// <remarks>
            /// A tag used to record fractions of seconds for the 'DateTime' tag.
            /// </remarks>
            internal static readonly MetadataKey SubSecTime = new(MetadataSection.Exif, 37520);

            /// <summary>
            /// Exif.SubSecTimeOriginal
            /// </summary>
            /// <remarks>
            /// A tag used to record fractions of seconds for the 'DateTimeOriginal' tag.
            /// </remarks>
            internal static readonly MetadataKey SubSecTimeOriginal = new(MetadataSection.Exif, 37521);

            /// <summary>
            /// Exif.SubSecTimeDigitized
            /// </summary>
            /// <remarks>
            /// A tag used to record fractions of seconds for the 'DateTimeDigitized' tag.
            /// </remarks>
            internal static readonly MetadataKey SubSecTimeDigitized = new(MetadataSection.Exif, 37522);

            /// <summary>
            /// Exif.FlashpixVersion
            /// </summary>
            /// <remarks>
            /// The FlashPix format version supported by a FPXR file.
            /// </remarks>
            internal static readonly MetadataKey FlashpixVersion = new(MetadataSection.Exif, 40960);

            /// <summary>
            /// Exif.ColorSpace
            /// </summary>
            /// <remarks>
            /// The color space information tag is always recorded as the color space specifier. Normally sRGB is used to define the color space based on the PC
            /// monitor conditions and environment. If a color space other than sRGB is used, Uncalibrated is set. Image data recorded as Uncalibrated can be treated
            /// as sRGB when it is converted to FlashPix.
            /// </remarks>
            internal static readonly MetadataKey ColorSpace = new(MetadataSection.Exif, 40961);

            /// <summary>
            /// Exif.PixelXDimension
            /// </summary>
            /// <remarks>
            /// Information specific to compressed data. When a compressed file is recorded, the valid width of the meaningful image must be recorded in this tag,
            /// whether or not there is padding data or a restart marker. This tag should not exist in an uncompressed file.
            /// </remarks>
            internal static readonly MetadataKey PixelXDimension = new(MetadataSection.Exif, 40962);

            /// <summary>
            /// Exif.PixelYDimension
            /// </summary>
            /// <remarks>
            /// Information specific to compressed data. When a compressed file is recorded, the valid height of the meaningful image must be recorded in this tag,
            /// whether or not there is padding data or a restart marker. This tag should not exist in an uncompressed file. Since data padding is unnecessary in the
            /// vertical direction, the number of lines recorded in this valid image height tag will in fact be the same as that recorded in the SOF.
            /// </remarks>
            internal static readonly MetadataKey PixelYDimension = new(MetadataSection.Exif, 40963);

            /// <summary>
            /// Exif.RelatedSoundFile
            /// </summary>
            /// <remarks>
            /// This tag is used to record the name of an audio file related to the image data. The only relational information recorded here is the Exif audio file
            /// name and extension (an ASCII string consisting of 8 characters + '.' + 3 characters). The path is not recorded.
            /// </remarks>
            internal static readonly MetadataKey RelatedSoundFile = new(MetadataSection.Exif, 40964);

            /// <summary>
            /// Exif.InteroperabilityTag
            /// </summary>
            /// <remarks>
            /// Interoperability IFD is composed of tags which stores the information to ensure the Interoperability and pointed by the following tag located in Exif
            /// IFD. The Interoperability structure of Interoperability IFD is the same as TIFF defined IFD structure but does not contain the image data
            /// characteristically compared with normal TIFF IFD.
            /// </remarks>
            internal static readonly MetadataKey InteroperabilityTag = new(MetadataSection.Exif, 40965);

            /// <summary>
            /// Exif.FlashEnergy
            /// </summary>
            /// <remarks>
            /// Indicates the strobe energy at the time the image is captured, as measured in Beam Candle Power Seconds (BCPS).
            /// </remarks>
            internal static readonly MetadataKey FlashEnergy = new(MetadataSection.Exif, 41483);

            /// <summary>
            /// Exif.SpatialFrequencyResponse
            /// </summary>
            /// <remarks>
            /// This tag records the camera or input device spatial frequency table and SFR values in the direction of image width, image height, and diagonal
            /// direction, as specified in ISO 12233.
            /// </remarks>
            internal static readonly MetadataKey SpatialFrequencyResponse = new(MetadataSection.Exif, 41484);

            /// <summary>
            /// Exif.FocalPlaneXResolution
            /// </summary>
            /// <remarks>
            /// Indicates the number of pixels in the image width (X) direction per 'FocalPlaneResolutionUnit' on the camera focal plane.
            /// </remarks>
            internal static readonly MetadataKey FocalPlaneXResolution = new(MetadataSection.Exif, 41486);

            /// <summary>
            /// Exif.FocalPlaneYResolution
            /// </summary>
            /// <remarks>
            /// Indicates the number of pixels in the image height (V) direction per 'FocalPlaneResolutionUnit' on the camera focal plane.
            /// </remarks>
            internal static readonly MetadataKey FocalPlaneYResolution = new(MetadataSection.Exif, 41487);

            /// <summary>
            /// Exif.FocalPlaneResolutionUnit
            /// </summary>
            /// <remarks>
            /// Indicates the unit for measuring 'FocalPlaneXResolution' and 'FocalPlaneYResolution'. This value is the same as the 'ResolutionUnit'.
            /// </remarks>
            internal static readonly MetadataKey FocalPlaneResolutionUnit = new(MetadataSection.Exif, 41488);

            /// <summary>
            /// Exif.SubjectLocation
            /// </summary>
            /// <remarks>
            /// Indicates the location of the main subject in the scene. The value of this tag represents the pixel at the center of the main subject relative to the
            /// left edge, prior to rotation processing as per the 'Rotation' tag. The first value indicates the X column number and second indicates the Y row
            /// number.
            /// </remarks>
            internal static readonly MetadataKey SubjectLocation = new(MetadataSection.Exif, 41492);

            /// <summary>
            /// Exif.ExposureIndex
            /// </summary>
            /// <remarks>
            /// Indicates the exposure index selected on the camera or input device at the time the image is captured.
            /// </remarks>
            internal static readonly MetadataKey ExposureIndex = new(MetadataSection.Exif, 41493);

            /// <summary>
            /// Exif.SensingMethod
            /// </summary>
            /// <remarks>
            /// Indicates the image sensor type on the camera or input device.
            /// </remarks>
            internal static readonly MetadataKey SensingMethod = new(MetadataSection.Exif, 41495);

            /// <summary>
            /// Exif.FileSource
            /// </summary>
            /// <remarks>
            /// Indicates the image source. If a DSC recorded the image, this tag value of this tag always be set to 3, indicating that the image was recorded on a
            /// DSC.
            /// </remarks>
            internal static readonly MetadataKey FileSource = new(MetadataSection.Exif, 41728);

            /// <summary>
            /// Exif.SceneType
            /// </summary>
            /// <remarks>
            /// Indicates the type of scene. If a DSC recorded the image, this tag value must always be set to 1, indicating that the image was directly photographed.
            /// </remarks>
            internal static readonly MetadataKey SceneType = new(MetadataSection.Exif, 41729);

            /// <summary>
            /// Exif.CFAPattern
            /// </summary>
            /// <remarks>
            /// Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used. It does not apply to all
            /// sensing methods.
            /// </remarks>
            internal static readonly MetadataKey CFAPattern = new(MetadataSection.Exif, 41730);

            /// <summary>
            /// Exif.CustomRendered
            /// </summary>
            /// <remarks>
            /// This tag indicates the use of special processing on image data, such as rendering geared to output. When special processing is performed, the reader
            /// is expected to disable or minimize any further processing.
            /// </remarks>
            internal static readonly MetadataKey CustomRendered = new(MetadataSection.Exif, 41985);

            /// <summary>
            /// Exif.ExposureMode
            /// </summary>
            /// <remarks>
            /// This tag indicates the exposure mode set when the image was shot. In auto-bracketing mode, the camera shoots a series of frames of the same scene at
            /// different exposure settings.
            /// </remarks>
            internal static readonly MetadataKey ExposureMode = new(MetadataSection.Exif, 41986);

            /// <summary>
            /// Exif.WhiteBalance
            /// </summary>
            /// <remarks>
            /// This tag indicates the white balance mode set when the image was shot.
            /// </remarks>
            internal static readonly MetadataKey WhiteBalance = new(MetadataSection.Exif, 41987);

            /// <summary>
            /// Exif.DigitalZoomRatio
            /// </summary>
            /// <remarks>
            /// This tag indicates the digital zoom ratio when the image was shot. If the numerator of the recorded value is 0, this indicates that digital zoom was
            /// not used.
            /// </remarks>
            internal static readonly MetadataKey DigitalZoomRatio = new(MetadataSection.Exif, 41988);

            /// <summary>
            /// Exif.FocalLengthIn35mmFilm
            /// </summary>
            /// <remarks>
            /// This tag indicates the equivalent focal length assuming a 35mm film camera, in mm. A value of 0 means the focal length is unknown. Note that this tag
            /// differs from the 'FocalLength' tag.
            /// </remarks>
            internal static readonly MetadataKey FocalLengthIn35mmFilm = new(MetadataSection.Exif, 41989);

            /// <summary>
            /// Exif.SceneCaptureType
            /// </summary>
            /// <remarks>
            /// This tag indicates the type of scene that was shot. It can also be used to record the mode in which the image was shot. Note that this differs from
            /// the 'SceneType' tag.
            /// </remarks>
            internal static readonly MetadataKey SceneCaptureType = new(MetadataSection.Exif, 41990);

            /// <summary>
            /// Exif.GainControl
            /// </summary>
            /// <remarks>
            /// This tag indicates the degree of overall image gain adjustment.
            /// </remarks>
            internal static readonly MetadataKey GainControl = new(MetadataSection.Exif, 41991);

            /// <summary>
            /// Exif.Contrast
            /// </summary>
            /// <remarks>
            /// This tag indicates the direction of contrast processing applied by the camera when the image was shot.
            /// </remarks>
            internal static readonly MetadataKey Contrast = new(MetadataSection.Exif, 41992);

            /// <summary>
            /// Exif.Saturation
            /// </summary>
            /// <remarks>
            /// This tag indicates the direction of saturation processing applied by the camera when the image was shot.
            /// </remarks>
            internal static readonly MetadataKey Saturation = new(MetadataSection.Exif, 41993);

            /// <summary>
            /// Exif.Sharpness
            /// </summary>
            /// <remarks>
            /// This tag indicates the direction of sharpness processing applied by the camera when the image was shot.
            /// </remarks>
            internal static readonly MetadataKey Sharpness = new(MetadataSection.Exif, 41994);

            /// <summary>
            /// Exif.DeviceSettingDescription
            /// </summary>
            /// <remarks>
            /// This tag indicates information on the picture-taking conditions of a particular camera model. The tag is used only to indicate the picture-taking
            /// conditions in the reader.
            /// </remarks>
            internal static readonly MetadataKey DeviceSettingDescription = new(MetadataSection.Exif, 41995);

            /// <summary>
            /// Exif.SubjectDistanceRange
            /// </summary>
            /// <remarks>
            /// This tag indicates the distance to the subject.
            /// </remarks>
            internal static readonly MetadataKey SubjectDistanceRange = new(MetadataSection.Exif, 41996);

            /// <summary>
            /// Exif.ImageUniqueID
            /// </summary>
            /// <remarks>
            /// This tag indicates an identifier assigned uniquely to each image. It is recorded as an ASCII string equivalent to hexadecimal notation and 128-bit
            /// fixed length.
            /// </remarks>
            internal static readonly MetadataKey ImageUniqueID = new(MetadataSection.Exif, 42016);

            /// <summary>
            /// Exif.CameraOwnerName
            /// </summary>
            /// <remarks>
            /// This tag records the owner of a camera used in photography as an ASCII string.
            /// </remarks>
            internal static readonly MetadataKey CameraOwnerName = new(MetadataSection.Exif, 42032);

            /// <summary>
            /// Exif.BodySerialNumber
            /// </summary>
            /// <remarks>
            /// This tag records the serial number of the body of the camera that was used in photography as an ASCII string.
            /// </remarks>
            internal static readonly MetadataKey BodySerialNumber = new(MetadataSection.Exif, 42033);

            /// <summary>
            /// Exif.LensSpecification
            /// </summary>
            /// <remarks>
            /// This tag notes minimum focal length, maximum focal length, minimum F number in the minimum focal length, and minimum F number in the maximum focal
            /// length, which are specification information for the lens that was used in photography. When the minimum F number is unknown, the notation is 0/0
            /// </remarks>
            internal static readonly MetadataKey LensSpecification = new(MetadataSection.Exif, 42034);

            /// <summary>
            /// Exif.LensMake
            /// </summary>
            /// <remarks>
            /// This tag records the lens manufactor as an ASCII string.
            /// </remarks>
            internal static readonly MetadataKey LensMake = new(MetadataSection.Exif, 42035);

            /// <summary>
            /// Exif.LensModel
            /// </summary>
            /// <remarks>
            /// This tag records the lens's model name and model number as an ASCII string.
            /// </remarks>
            internal static readonly MetadataKey LensModel = new(MetadataSection.Exif, 42036);

            /// <summary>
            /// Exif.LensSerialNumber
            /// </summary>
            /// <remarks>
            /// This tag records the serial number of the interchangeable lens that was used in photography as an ASCII string.
            /// </remarks>
            internal static readonly MetadataKey LensSerialNumber = new(MetadataSection.Exif, 42037);
        }

        internal static class Interop
        {
            /// <summary>
            /// Interop.InteroperabilityIndex
            /// </summary>
            /// <remarks>
            /// Indicates the identification of the Interoperability rule. Use "R98" for stating ExifR98 Rules. Four bytes used including the termination code (NULL).
            /// see the separate volume of Recommended Exif Interoperability Rules (ExifR98) for other tags used for ExifR98.
            /// </remarks>
            internal static readonly MetadataKey InteroperabilityIndex = new(MetadataSection.Interop, 1);

            /// <summary>
            /// Interop.InteroperabilityVersion
            /// </summary>
            /// <remarks>
            /// Interoperability version
            /// </remarks>
            internal static readonly MetadataKey InteroperabilityVersion = new(MetadataSection.Interop, 2);

            /// <summary>
            /// Interop.RelatedImageFileFormat
            /// </summary>
            /// <remarks>
            /// File format of image file
            /// </remarks>
            internal static readonly MetadataKey RelatedImageFileFormat = new(MetadataSection.Interop, 4096);

            /// <summary>
            /// Interop.RelatedImageWidth
            /// </summary>
            /// <remarks>
            /// Image width
            /// </remarks>
            internal static readonly MetadataKey RelatedImageWidth = new(MetadataSection.Interop, 4097);

            /// <summary>
            /// Interop.RelatedImageLength
            /// </summary>
            /// <remarks>
            /// Image height
            /// </remarks>
            internal static readonly MetadataKey RelatedImageLength = new(MetadataSection.Interop, 4098);
        }

        internal static class Gps
        {
            /// <summary>
            /// Gps.GPSVersionID
            /// </summary>
            /// <remarks>
            /// Indicates the version of 'GPSInfoIFD'. The version is given as 2.0.0.0. This tag is mandatory when 'GPSInfo' tag is present. (Note: The 'GPSVersionID'
            /// tag is given in bytes, unlike the 'ExifVersion' tag. When the version is 2.0.0.0, the tag value is 02000000.H).
            /// </remarks>
            internal static readonly MetadataKey GPSVersionID = new(MetadataSection.Gps, 0);

            /// <summary>
            /// Gps.GPSLatitudeRef
            /// </summary>
            /// <remarks>
            /// Indicates whether the latitude is north or south latitude. The ASCII value 'N' indicates north latitude, and 'S' is south latitude.
            /// </remarks>
            internal static readonly MetadataKey GPSLatitudeRef = new(MetadataSection.Gps, 1);

            /// <summary>
            /// Gps.GPSLatitude
            /// </summary>
            /// <remarks>
            /// Indicates the latitude. The latitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. When degrees,
            /// minutes and seconds are expressed, the format is dd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given up
            /// to two decimal places, the format is dd/1,mmmm/100,0/1.
            /// </remarks>
            internal static readonly MetadataKey GPSLatitude = new(MetadataSection.Gps, 2);

            /// <summary>
            /// Gps.GPSLongitudeRef
            /// </summary>
            /// <remarks>
            /// Indicates whether the longitude is east or west longitude. ASCII 'E' indicates east longitude, and 'W' is west longitude.
            /// </remarks>
            internal static readonly MetadataKey GPSLongitudeRef = new(MetadataSection.Gps, 3);

            /// <summary>
            /// Gps.GPSLongitude
            /// </summary>
            /// <remarks>
            /// Indicates the longitude. The longitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. When degrees,
            /// minutes and seconds are expressed, the format is ddd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given
            /// up to two decimal places, the format is ddd/1,mmmm/100,0/1.
            /// </remarks>
            internal static readonly MetadataKey GPSLongitude = new(MetadataSection.Gps, 4);

            /// <summary>
            /// Gps.GPSAltitudeRef
            /// </summary>
            /// <remarks>
            /// Indicates the altitude used as the reference altitude. If the reference is sea level and the altitude is above sea level, 0 is given. If the altitude
            /// is below sea level, a value of 1 is given and the altitude is indicated as an absolute value in the GSPAltitude tag. The reference unit is meters.
            /// Note that this tag is BYTE type, unlike other reference tags.
            /// </remarks>
            internal static readonly MetadataKey GPSAltitudeRef = new(MetadataSection.Gps, 5);

            /// <summary>
            /// Gps.GPSAltitude
            /// </summary>
            /// <remarks>
            /// Indicates the altitude based on the reference in GPSAltitudeRef. Altitude is expressed as one RATIONAL value. The reference unit is meters.
            /// </remarks>
            internal static readonly MetadataKey GPSAltitude = new(MetadataSection.Gps, 6);

            /// <summary>
            /// Gps.GPSTimeStamp
            /// </summary>
            /// <remarks>
            /// Indicates the time as UTC (Coordinated Universal Time). 'TimeStamp' is expressed as three RATIONAL values giving the hour, minute, and second (atomic
            /// clock).
            /// </remarks>
            internal static readonly MetadataKey GPSTimeStamp = new(MetadataSection.Gps, 7);

            /// <summary>
            /// Gps.GPSSatellites
            /// </summary>
            /// <remarks>
            /// Indicates the GPS satellites used for measurements. This tag can be used to describe the number of satellites, their ID number, angle of elevation,
            /// azimuth, SNR and other information in ASCII notation. The format is not specified. If the GPS receiver is incapable of taking measurements, value of
            /// the tag is set to NULL.
            /// </remarks>
            internal static readonly MetadataKey GPSSatellites = new(MetadataSection.Gps, 8);

            /// <summary>
            /// Gps.GPSStatus
            /// </summary>
            /// <remarks>
            /// Indicates the status of the GPS receiver when the image is recorded. "A" means measurement is in progress, and "V" means the measurement is
            /// Interoperability.
            /// </remarks>
            internal static readonly MetadataKey GPSStatus = new(MetadataSection.Gps, 9);

            /// <summary>
            /// Gps.GPSMeasureMode
            /// </summary>
            /// <remarks>
            /// Indicates the GPS measurement mode. "2" means two-dimensional measurement and "3" means three-dimensional measurement is in progress.
            /// </remarks>
            internal static readonly MetadataKey GPSMeasureMode = new(MetadataSection.Gps, 10);

            /// <summary>
            /// Gps.GPSDOP
            /// </summary>
            /// <remarks>
            /// Indicates the GPS DOP (data degree of precision). An HDOP value is written during two-dimensional measurement, and PDOP during three-dimensional
            /// measurement.
            /// </remarks>
            internal static readonly MetadataKey GPSDOP = new(MetadataSection.Gps, 11);

            /// <summary>
            /// Gps.GPSSpeedRef
            /// </summary>
            /// <remarks>
            /// Indicates the unit used to express the GPS receiver speed of movement. "K" "M" and "N" represents kilometers per hour, miles per hour, and knots.
            /// </remarks>
            internal static readonly MetadataKey GPSSpeedRef = new(MetadataSection.Gps, 12);

            /// <summary>
            /// Gps.GPSSpeed
            /// </summary>
            /// <remarks>
            /// Indicates the speed of GPS receiver movement.
            /// </remarks>
            internal static readonly MetadataKey GPSSpeed = new(MetadataSection.Gps, 13);

            /// <summary>
            /// Gps.GPSTrackRef
            /// </summary>
            /// <remarks>
            /// Indicates the reference for giving the direction of GPS receiver movement. "T" denotes true direction and "M" is magnetic direction.
            /// </remarks>
            internal static readonly MetadataKey GPSTrackRef = new(MetadataSection.Gps, 14);

            /// <summary>
            /// Gps.GPSTrack
            /// </summary>
            /// <remarks>
            /// Indicates the direction of GPS receiver movement. The range of values is from 0.00 to 359.99.
            /// </remarks>
            internal static readonly MetadataKey GPSTrack = new(MetadataSection.Gps, 15);

            /// <summary>
            /// Gps.GPSImgDirectionRef
            /// </summary>
            /// <remarks>
            /// Indicates the reference for giving the direction of the image when it is captured. "T" denotes true direction and "M" is magnetic direction.
            /// </remarks>
            internal static readonly MetadataKey GPSImgDirectionRef = new(MetadataSection.Gps, 16);

            /// <summary>
            /// Gps.GPSImgDirection
            /// </summary>
            /// <remarks>
            /// Indicates the direction of the image when it was captured. The range of values is from 0.00 to 359.99.
            /// </remarks>
            internal static readonly MetadataKey GPSImgDirection = new(MetadataSection.Gps, 17);

            /// <summary>
            /// Gps.GPSMapDatum
            /// </summary>
            /// <remarks>
            /// Indicates the geodetic survey data used by the GPS receiver. If the survey data is restricted to Japan, the value of this tag is "TOKYO" or "WGS-84".
            /// </remarks>
            internal static readonly MetadataKey GPSMapDatum = new(MetadataSection.Gps, 18);

            /// <summary>
            /// Gps.GPSDestLatitudeRef
            /// </summary>
            /// <remarks>
            /// Indicates whether the latitude of the destination point is north or south latitude. The ASCII value "N" indicates north latitude, and "S" is south
            /// latitude.
            /// </remarks>
            internal static readonly MetadataKey GPSDestLatitudeRef = new(MetadataSection.Gps, 19);

            /// <summary>
            /// Gps.GPSDestLatitude
            /// </summary>
            /// <remarks>
            /// Indicates the latitude of the destination point. The latitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds,
            /// respectively. If latitude is expressed as degrees, minutes and seconds, a typical format would be dd/1,mm/1,ss/1. When degrees and minutes are used
            /// and, for example, fractions of minutes are given up to two decimal places, the format would be dd/1,mmmm/100,0/1.
            /// </remarks>
            internal static readonly MetadataKey GPSDestLatitude = new(MetadataSection.Gps, 20);

            /// <summary>
            /// Gps.GPSDestLongitudeRef
            /// </summary>
            /// <remarks>
            /// Indicates whether the longitude of the destination point is east or west longitude. ASCII "E" indicates east longitude, and "W" is west longitude.
            /// </remarks>
            internal static readonly MetadataKey GPSDestLongitudeRef = new(MetadataSection.Gps, 21);

            /// <summary>
            /// Gps.GPSDestLongitude
            /// </summary>
            /// <remarks>
            /// Indicates the longitude of the destination point. The longitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds,
            /// respectively. If longitude is expressed as degrees, minutes and seconds, a typical format would be ddd/1,mm/1,ss/1. When degrees and minutes are used
            /// and, for example, fractions of minutes are given up to two decimal places, the format would be ddd/1,mmmm/100,0/1.
            /// </remarks>
            internal static readonly MetadataKey GPSDestLongitude = new(MetadataSection.Gps, 22);

            /// <summary>
            /// Gps.GPSDestBearingRef
            /// </summary>
            /// <remarks>
            /// Indicates the reference used for giving the bearing to the destination point. "T" denotes true direction and "M" is magnetic direction.
            /// </remarks>
            internal static readonly MetadataKey GPSDestBearingRef = new(MetadataSection.Gps, 23);

            /// <summary>
            /// Gps.GPSDestBearing
            /// </summary>
            /// <remarks>
            /// Indicates the bearing to the destination point. The range of values is from 0.00 to 359.99.
            /// </remarks>
            internal static readonly MetadataKey GPSDestBearing = new(MetadataSection.Gps, 24);

            /// <summary>
            /// Gps.GPSDestDistanceRef
            /// </summary>
            /// <remarks>
            /// Indicates the unit used to express the distance to the destination point. "K", "M" and "N" represent kilometers, miles and knots.
            /// </remarks>
            internal static readonly MetadataKey GPSDestDistanceRef = new(MetadataSection.Gps, 25);

            /// <summary>
            /// Gps.GPSDestDistance
            /// </summary>
            /// <remarks>
            /// Indicates the distance to the destination point.
            /// </remarks>
            internal static readonly MetadataKey GPSDestDistance = new(MetadataSection.Gps, 26);

            /// <summary>
            /// Gps.GPSProcessingMethod
            /// </summary>
            /// <remarks>
            /// A character string recording the name of the method used for location finding. The first byte indicates the character code used, and this is followed
            /// by the name of the method.
            /// </remarks>
            internal static readonly MetadataKey GPSProcessingMethod = new(MetadataSection.Gps, 27);

            /// <summary>
            /// Gps.GPSAreaInformation
            /// </summary>
            /// <remarks>
            /// A character string recording the name of the GPS area. The first byte indicates the character code used, and this is followed by the name of the GPS
            /// area.
            /// </remarks>
            internal static readonly MetadataKey GPSAreaInformation = new(MetadataSection.Gps, 28);

            /// <summary>
            /// Gps.GPSDateStamp
            /// </summary>
            /// <remarks>
            /// A character string recording date and time information relative to UTC (Coordinated Universal Time). The format is "YYYY:MM:DD.".
            /// </remarks>
            internal static readonly MetadataKey GPSDateStamp = new(MetadataSection.Gps, 29);

            /// <summary>
            /// Gps.GPSDifferential
            /// </summary>
            /// <remarks>
            /// Indicates whether differential correction is applied to the GPS receiver.
            /// </remarks>
            internal static readonly MetadataKey GPSDifferential = new(MetadataSection.Gps, 30);

            /// <summary>
            /// Gps.GPSHPositioningError
            /// </summary>
            /// <remarks>
            /// Indicates the horizontal positioning errors in meters.
            /// </remarks>
            internal static readonly MetadataKey GPSHPositioningError = new(MetadataSection.Gps, 31);
        }
    }
}
