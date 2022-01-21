﻿////////////////////////////////////////////////////////////////////////
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

using System.Collections.ObjectModel;

namespace WebPFileType.Exif
{
    internal static class MetadataKeyTable
    {
        internal static readonly ReadOnlyCollection<MetadataKey> Rows = new ReadOnlyCollection<MetadataKey>(CreateRows());

        private static MetadataKey[] CreateRows()
        {
            // Generated from https://www.exiv2.org/tags.html
            return new MetadataKey[]
                {
                    new MetadataKey(MetadataSection.Image, 11), // ProcessingSoftware
                    new MetadataKey(MetadataSection.Image, 254), // NewSubfileType
                    new MetadataKey(MetadataSection.Image, 255), // SubfileType
                    new MetadataKey(MetadataSection.Image, 256), // ImageWidth
                    new MetadataKey(MetadataSection.Image, 257), // ImageLength
                    new MetadataKey(MetadataSection.Image, 258), // BitsPerSample
                    new MetadataKey(MetadataSection.Image, 259), // Compression
                    new MetadataKey(MetadataSection.Image, 262), // PhotometricInterpretation
                    new MetadataKey(MetadataSection.Image, 263), // Thresholding
                    new MetadataKey(MetadataSection.Image, 264), // CellWidth
                    new MetadataKey(MetadataSection.Image, 265), // CellLength
                    new MetadataKey(MetadataSection.Image, 266), // FillOrder
                    new MetadataKey(MetadataSection.Image, 269), // DocumentName
                    new MetadataKey(MetadataSection.Image, 270), // ImageDescription
                    new MetadataKey(MetadataSection.Image, 271), // Make
                    new MetadataKey(MetadataSection.Image, 272), // Model
                    new MetadataKey(MetadataSection.Image, 273), // StripOffsets
                    new MetadataKey(MetadataSection.Image, 274), // Orientation
                    new MetadataKey(MetadataSection.Image, 277), // SamplesPerPixel
                    new MetadataKey(MetadataSection.Image, 278), // RowsPerStrip
                    new MetadataKey(MetadataSection.Image, 279), // StripByteCounts
                    new MetadataKey(MetadataSection.Image, 282), // XResolution
                    new MetadataKey(MetadataSection.Image, 283), // YResolution
                    new MetadataKey(MetadataSection.Image, 284), // PlanarConfiguration
                    new MetadataKey(MetadataSection.Image, 290), // GrayResponseUnit
                    new MetadataKey(MetadataSection.Image, 291), // GrayResponseCurve
                    new MetadataKey(MetadataSection.Image, 292), // T4Options
                    new MetadataKey(MetadataSection.Image, 293), // T6Options
                    new MetadataKey(MetadataSection.Image, 296), // ResolutionUnit
                    new MetadataKey(MetadataSection.Image, 297), // PageNumber
                    new MetadataKey(MetadataSection.Image, 301), // TransferFunction
                    new MetadataKey(MetadataSection.Image, 305), // Software
                    new MetadataKey(MetadataSection.Image, 306), // DateTime
                    new MetadataKey(MetadataSection.Image, 315), // Artist
                    new MetadataKey(MetadataSection.Image, 316), // HostComputer
                    new MetadataKey(MetadataSection.Image, 317), // Predictor
                    new MetadataKey(MetadataSection.Image, 318), // WhitePoint
                    new MetadataKey(MetadataSection.Image, 319), // PrimaryChromaticities
                    new MetadataKey(MetadataSection.Image, 320), // ColorMap
                    new MetadataKey(MetadataSection.Image, 321), // HalftoneHints
                    new MetadataKey(MetadataSection.Image, 322), // TileWidth
                    new MetadataKey(MetadataSection.Image, 323), // TileLength
                    new MetadataKey(MetadataSection.Image, 324), // TileOffsets
                    new MetadataKey(MetadataSection.Image, 325), // TileByteCounts
                    new MetadataKey(MetadataSection.Image, 330), // SubIFDs
                    new MetadataKey(MetadataSection.Image, 332), // InkSet
                    new MetadataKey(MetadataSection.Image, 333), // InkNames
                    new MetadataKey(MetadataSection.Image, 334), // NumberOfInks
                    new MetadataKey(MetadataSection.Image, 336), // DotRange
                    new MetadataKey(MetadataSection.Image, 337), // TargetPrinter
                    new MetadataKey(MetadataSection.Image, 338), // ExtraSamples
                    new MetadataKey(MetadataSection.Image, 339), // SampleFormat
                    new MetadataKey(MetadataSection.Image, 340), // SMinSampleValue
                    new MetadataKey(MetadataSection.Image, 341), // SMaxSampleValue
                    new MetadataKey(MetadataSection.Image, 342), // TransferRange
                    new MetadataKey(MetadataSection.Image, 343), // ClipPath
                    new MetadataKey(MetadataSection.Image, 344), // XClipPathUnits
                    new MetadataKey(MetadataSection.Image, 345), // YClipPathUnits
                    new MetadataKey(MetadataSection.Image, 346), // Indexed
                    new MetadataKey(MetadataSection.Image, 347), // JPEGTables
                    new MetadataKey(MetadataSection.Image, 351), // OPIProxy
                    new MetadataKey(MetadataSection.Image, 512), // JPEGProc
                    new MetadataKey(MetadataSection.Image, 513), // JPEGInterchangeFormat
                    new MetadataKey(MetadataSection.Image, 514), // JPEGInterchangeFormatLength
                    new MetadataKey(MetadataSection.Image, 515), // JPEGRestartInterval
                    new MetadataKey(MetadataSection.Image, 517), // JPEGLosslessPredictors
                    new MetadataKey(MetadataSection.Image, 518), // JPEGPointTransforms
                    new MetadataKey(MetadataSection.Image, 519), // JPEGQTables
                    new MetadataKey(MetadataSection.Image, 520), // JPEGDCTables
                    new MetadataKey(MetadataSection.Image, 521), // JPEGACTables
                    new MetadataKey(MetadataSection.Image, 529), // YCbCrCoefficients
                    new MetadataKey(MetadataSection.Image, 530), // YCbCrSubSampling
                    new MetadataKey(MetadataSection.Image, 531), // YCbCrPositioning
                    new MetadataKey(MetadataSection.Image, 532), // ReferenceBlackWhite
                    new MetadataKey(MetadataSection.Image, 700), // XMLPacket
                    new MetadataKey(MetadataSection.Image, 18246), // Rating
                    new MetadataKey(MetadataSection.Image, 18249), // RatingPercent
                    new MetadataKey(MetadataSection.Image, 32781), // ImageID
                    new MetadataKey(MetadataSection.Image, 33421), // CFARepeatPatternDim
                    new MetadataKey(MetadataSection.Image, 33422), // CFAPattern
                    new MetadataKey(MetadataSection.Image, 33423), // BatteryLevel
                    new MetadataKey(MetadataSection.Image, 33432), // Copyright
                    new MetadataKey(MetadataSection.Image, 33434), // ExposureTime
                    new MetadataKey(MetadataSection.Image, 33437), // FNumber
                    new MetadataKey(MetadataSection.Image, 33723), // IPTCNAA
                    new MetadataKey(MetadataSection.Image, 34377), // ImageResources
                    new MetadataKey(MetadataSection.Image, 34665), // ExifTag
                    new MetadataKey(MetadataSection.Image, 34675), // InterColorProfile
                    new MetadataKey(MetadataSection.Image, 34850), // ExposureProgram
                    new MetadataKey(MetadataSection.Image, 34852), // SpectralSensitivity
                    new MetadataKey(MetadataSection.Image, 34853), // GPSTag
                    new MetadataKey(MetadataSection.Image, 34855), // ISOSpeedRatings
                    new MetadataKey(MetadataSection.Image, 34856), // OECF
                    new MetadataKey(MetadataSection.Image, 34857), // Interlace
                    new MetadataKey(MetadataSection.Image, 34858), // TimeZoneOffset
                    new MetadataKey(MetadataSection.Image, 34859), // SelfTimerMode
                    new MetadataKey(MetadataSection.Image, 36867), // DateTimeOriginal
                    new MetadataKey(MetadataSection.Image, 37122), // CompressedBitsPerPixel
                    new MetadataKey(MetadataSection.Image, 37377), // ShutterSpeedValue
                    new MetadataKey(MetadataSection.Image, 37378), // ApertureValue
                    new MetadataKey(MetadataSection.Image, 37379), // BrightnessValue
                    new MetadataKey(MetadataSection.Image, 37380), // ExposureBiasValue
                    new MetadataKey(MetadataSection.Image, 37381), // MaxApertureValue
                    new MetadataKey(MetadataSection.Image, 37382), // SubjectDistance
                    new MetadataKey(MetadataSection.Image, 37383), // MeteringMode
                    new MetadataKey(MetadataSection.Image, 37384), // LightSource
                    new MetadataKey(MetadataSection.Image, 37385), // Flash
                    new MetadataKey(MetadataSection.Image, 37386), // FocalLength
                    new MetadataKey(MetadataSection.Image, 37387), // FlashEnergy
                    new MetadataKey(MetadataSection.Image, 37388), // SpatialFrequencyResponse
                    new MetadataKey(MetadataSection.Image, 37389), // Noise
                    new MetadataKey(MetadataSection.Image, 37390), // FocalPlaneXResolution
                    new MetadataKey(MetadataSection.Image, 37391), // FocalPlaneYResolution
                    new MetadataKey(MetadataSection.Image, 37392), // FocalPlaneResolutionUnit
                    new MetadataKey(MetadataSection.Image, 37393), // ImageNumber
                    new MetadataKey(MetadataSection.Image, 37394), // SecurityClassification
                    new MetadataKey(MetadataSection.Image, 37395), // ImageHistory
                    new MetadataKey(MetadataSection.Image, 37396), // SubjectLocation
                    new MetadataKey(MetadataSection.Image, 37397), // ExposureIndex
                    new MetadataKey(MetadataSection.Image, 37398), // TIFFEPStandardID
                    new MetadataKey(MetadataSection.Image, 37399), // SensingMethod
                    new MetadataKey(MetadataSection.Image, 40091), // XPTitle
                    new MetadataKey(MetadataSection.Image, 40092), // XPComment
                    new MetadataKey(MetadataSection.Image, 40093), // XPAuthor
                    new MetadataKey(MetadataSection.Image, 40094), // XPKeywords
                    new MetadataKey(MetadataSection.Image, 40095), // XPSubject
                    new MetadataKey(MetadataSection.Image, 50341), // PrintImageMatching
                    new MetadataKey(MetadataSection.Image, 50706), // DNGVersion
                    new MetadataKey(MetadataSection.Image, 50707), // DNGBackwardVersion
                    new MetadataKey(MetadataSection.Image, 50708), // UniqueCameraModel
                    new MetadataKey(MetadataSection.Image, 50709), // LocalizedCameraModel
                    new MetadataKey(MetadataSection.Image, 50710), // CFAPlaneColor
                    new MetadataKey(MetadataSection.Image, 50711), // CFALayout
                    new MetadataKey(MetadataSection.Image, 50712), // LinearizationTable
                    new MetadataKey(MetadataSection.Image, 50713), // BlackLevelRepeatDim
                    new MetadataKey(MetadataSection.Image, 50714), // BlackLevel
                    new MetadataKey(MetadataSection.Image, 50715), // BlackLevelDeltaH
                    new MetadataKey(MetadataSection.Image, 50716), // BlackLevelDeltaV
                    new MetadataKey(MetadataSection.Image, 50717), // WhiteLevel
                    new MetadataKey(MetadataSection.Image, 50718), // DefaultScale
                    new MetadataKey(MetadataSection.Image, 50719), // DefaultCropOrigin
                    new MetadataKey(MetadataSection.Image, 50720), // DefaultCropSize
                    new MetadataKey(MetadataSection.Image, 50721), // ColorMatrix1
                    new MetadataKey(MetadataSection.Image, 50722), // ColorMatrix2
                    new MetadataKey(MetadataSection.Image, 50723), // CameraCalibration1
                    new MetadataKey(MetadataSection.Image, 50724), // CameraCalibration2
                    new MetadataKey(MetadataSection.Image, 50725), // ReductionMatrix1
                    new MetadataKey(MetadataSection.Image, 50726), // ReductionMatrix2
                    new MetadataKey(MetadataSection.Image, 50727), // AnalogBalance
                    new MetadataKey(MetadataSection.Image, 50728), // AsShotNeutral
                    new MetadataKey(MetadataSection.Image, 50729), // AsShotWhiteXY
                    new MetadataKey(MetadataSection.Image, 50730), // BaselineExposure
                    new MetadataKey(MetadataSection.Image, 50731), // BaselineNoise
                    new MetadataKey(MetadataSection.Image, 50732), // BaselineSharpness
                    new MetadataKey(MetadataSection.Image, 50733), // BayerGreenSplit
                    new MetadataKey(MetadataSection.Image, 50734), // LinearResponseLimit
                    new MetadataKey(MetadataSection.Image, 50735), // CameraSerialNumber
                    new MetadataKey(MetadataSection.Image, 50736), // LensInfo
                    new MetadataKey(MetadataSection.Image, 50737), // ChromaBlurRadius
                    new MetadataKey(MetadataSection.Image, 50738), // AntiAliasStrength
                    new MetadataKey(MetadataSection.Image, 50739), // ShadowScale
                    new MetadataKey(MetadataSection.Image, 50740), // DNGPrivateData
                    new MetadataKey(MetadataSection.Image, 50741), // MakerNoteSafety
                    new MetadataKey(MetadataSection.Image, 50778), // CalibrationIlluminant1
                    new MetadataKey(MetadataSection.Image, 50779), // CalibrationIlluminant2
                    new MetadataKey(MetadataSection.Image, 50780), // BestQualityScale
                    new MetadataKey(MetadataSection.Image, 50781), // RawDataUniqueID
                    new MetadataKey(MetadataSection.Image, 50827), // OriginalRawFileName
                    new MetadataKey(MetadataSection.Image, 50828), // OriginalRawFileData
                    new MetadataKey(MetadataSection.Image, 50829), // ActiveArea
                    new MetadataKey(MetadataSection.Image, 50830), // MaskedAreas
                    new MetadataKey(MetadataSection.Image, 50831), // AsShotICCProfile
                    new MetadataKey(MetadataSection.Image, 50832), // AsShotPreProfileMatrix
                    new MetadataKey(MetadataSection.Image, 50833), // CurrentICCProfile
                    new MetadataKey(MetadataSection.Image, 50834), // CurrentPreProfileMatrix
                    new MetadataKey(MetadataSection.Image, 50879), // ColorimetricReference
                    new MetadataKey(MetadataSection.Image, 50931), // CameraCalibrationSignature
                    new MetadataKey(MetadataSection.Image, 50932), // ProfileCalibrationSignature
                    new MetadataKey(MetadataSection.Image, 50934), // AsShotProfileName
                    new MetadataKey(MetadataSection.Image, 50935), // NoiseReductionApplied
                    new MetadataKey(MetadataSection.Image, 50936), // ProfileName
                    new MetadataKey(MetadataSection.Image, 50937), // ProfileHueSatMapDims
                    new MetadataKey(MetadataSection.Image, 50938), // ProfileHueSatMapData1
                    new MetadataKey(MetadataSection.Image, 50939), // ProfileHueSatMapData2
                    new MetadataKey(MetadataSection.Image, 50940), // ProfileToneCurve
                    new MetadataKey(MetadataSection.Image, 50941), // ProfileEmbedPolicy
                    new MetadataKey(MetadataSection.Image, 50942), // ProfileCopyright
                    new MetadataKey(MetadataSection.Image, 50964), // ForwardMatrix1
                    new MetadataKey(MetadataSection.Image, 50965), // ForwardMatrix2
                    new MetadataKey(MetadataSection.Image, 50966), // PreviewApplicationName
                    new MetadataKey(MetadataSection.Image, 50967), // PreviewApplicationVersion
                    new MetadataKey(MetadataSection.Image, 50968), // PreviewSettingsName
                    new MetadataKey(MetadataSection.Image, 50969), // PreviewSettingsDigest
                    new MetadataKey(MetadataSection.Image, 50970), // PreviewColorSpace
                    new MetadataKey(MetadataSection.Image, 50971), // PreviewDateTime
                    new MetadataKey(MetadataSection.Image, 50972), // RawImageDigest
                    new MetadataKey(MetadataSection.Image, 50973), // OriginalRawFileDigest
                    new MetadataKey(MetadataSection.Image, 50974), // SubTileBlockSize
                    new MetadataKey(MetadataSection.Image, 50975), // RowInterleaveFactor
                    new MetadataKey(MetadataSection.Image, 50981), // ProfileLookTableDims
                    new MetadataKey(MetadataSection.Image, 50982), // ProfileLookTableData
                    new MetadataKey(MetadataSection.Image, 51008), // OpcodeList1
                    new MetadataKey(MetadataSection.Image, 51009), // OpcodeList2
                    new MetadataKey(MetadataSection.Image, 51022), // OpcodeList3
                    new MetadataKey(MetadataSection.Image, 51041), // NoiseProfile
                    new MetadataKey(MetadataSection.Image, 51043), // TimeCodes
                    new MetadataKey(MetadataSection.Image, 51044), // FrameRate
                    new MetadataKey(MetadataSection.Image, 51058), // TStop
                    new MetadataKey(MetadataSection.Image, 51081), // ReelName
                    new MetadataKey(MetadataSection.Image, 51105), // CameraLabel
                    new MetadataKey(MetadataSection.Exif, 33434), // ExposureTime
                    new MetadataKey(MetadataSection.Exif, 33437), // FNumber
                    new MetadataKey(MetadataSection.Exif, 34850), // ExposureProgram
                    new MetadataKey(MetadataSection.Exif, 34852), // SpectralSensitivity
                    new MetadataKey(MetadataSection.Exif, 34855), // ISOSpeedRatings
                    new MetadataKey(MetadataSection.Exif, 34856), // OECF
                    new MetadataKey(MetadataSection.Exif, 34864), // SensitivityType
                    new MetadataKey(MetadataSection.Exif, 34865), // StandardOutputSensitivity
                    new MetadataKey(MetadataSection.Exif, 34866), // RecommendedExposureIndex
                    new MetadataKey(MetadataSection.Exif, 34867), // ISOSpeed
                    new MetadataKey(MetadataSection.Exif, 34868), // ISOSpeedLatitudeyyy
                    new MetadataKey(MetadataSection.Exif, 34869), // ISOSpeedLatitudezzz
                    new MetadataKey(MetadataSection.Exif, 36864), // ExifVersion
                    new MetadataKey(MetadataSection.Exif, 36867), // DateTimeOriginal
                    new MetadataKey(MetadataSection.Exif, 36868), // DateTimeDigitized
                    new MetadataKey(MetadataSection.Exif, 37121), // ComponentsConfiguration
                    new MetadataKey(MetadataSection.Exif, 37122), // CompressedBitsPerPixel
                    new MetadataKey(MetadataSection.Exif, 37377), // ShutterSpeedValue
                    new MetadataKey(MetadataSection.Exif, 37378), // ApertureValue
                    new MetadataKey(MetadataSection.Exif, 37379), // BrightnessValue
                    new MetadataKey(MetadataSection.Exif, 37380), // ExposureBiasValue
                    new MetadataKey(MetadataSection.Exif, 37381), // MaxApertureValue
                    new MetadataKey(MetadataSection.Exif, 37382), // SubjectDistance
                    new MetadataKey(MetadataSection.Exif, 37383), // MeteringMode
                    new MetadataKey(MetadataSection.Exif, 37384), // LightSource
                    new MetadataKey(MetadataSection.Exif, 37385), // Flash
                    new MetadataKey(MetadataSection.Exif, 37386), // FocalLength
                    new MetadataKey(MetadataSection.Exif, 37396), // SubjectArea
                    new MetadataKey(MetadataSection.Exif, 37500), // MakerNote
                    new MetadataKey(MetadataSection.Exif, 37510), // UserComment
                    new MetadataKey(MetadataSection.Exif, 37520), // SubSecTime
                    new MetadataKey(MetadataSection.Exif, 37521), // SubSecTimeOriginal
                    new MetadataKey(MetadataSection.Exif, 37522), // SubSecTimeDigitized
                    new MetadataKey(MetadataSection.Exif, 40960), // FlashpixVersion
                    new MetadataKey(MetadataSection.Exif, 40961), // ColorSpace
                    new MetadataKey(MetadataSection.Exif, 40962), // PixelXDimension
                    new MetadataKey(MetadataSection.Exif, 40963), // PixelYDimension
                    new MetadataKey(MetadataSection.Exif, 40964), // RelatedSoundFile
                    new MetadataKey(MetadataSection.Exif, 40965), // InteroperabilityTag
                    new MetadataKey(MetadataSection.Exif, 41483), // FlashEnergy
                    new MetadataKey(MetadataSection.Exif, 41484), // SpatialFrequencyResponse
                    new MetadataKey(MetadataSection.Exif, 41486), // FocalPlaneXResolution
                    new MetadataKey(MetadataSection.Exif, 41487), // FocalPlaneYResolution
                    new MetadataKey(MetadataSection.Exif, 41488), // FocalPlaneResolutionUnit
                    new MetadataKey(MetadataSection.Exif, 41492), // SubjectLocation
                    new MetadataKey(MetadataSection.Exif, 41493), // ExposureIndex
                    new MetadataKey(MetadataSection.Exif, 41495), // SensingMethod
                    new MetadataKey(MetadataSection.Exif, 41728), // FileSource
                    new MetadataKey(MetadataSection.Exif, 41729), // SceneType
                    new MetadataKey(MetadataSection.Exif, 41730), // CFAPattern
                    new MetadataKey(MetadataSection.Exif, 41985), // CustomRendered
                    new MetadataKey(MetadataSection.Exif, 41986), // ExposureMode
                    new MetadataKey(MetadataSection.Exif, 41987), // WhiteBalance
                    new MetadataKey(MetadataSection.Exif, 41988), // DigitalZoomRatio
                    new MetadataKey(MetadataSection.Exif, 41989), // FocalLengthIn35mmFilm
                    new MetadataKey(MetadataSection.Exif, 41990), // SceneCaptureType
                    new MetadataKey(MetadataSection.Exif, 41991), // GainControl
                    new MetadataKey(MetadataSection.Exif, 41992), // Contrast
                    new MetadataKey(MetadataSection.Exif, 41993), // Saturation
                    new MetadataKey(MetadataSection.Exif, 41994), // Sharpness
                    new MetadataKey(MetadataSection.Exif, 41995), // DeviceSettingDescription
                    new MetadataKey(MetadataSection.Exif, 41996), // SubjectDistanceRange
                    new MetadataKey(MetadataSection.Exif, 42016), // ImageUniqueID
                    new MetadataKey(MetadataSection.Exif, 42032), // CameraOwnerName
                    new MetadataKey(MetadataSection.Exif, 42033), // BodySerialNumber
                    new MetadataKey(MetadataSection.Exif, 42034), // LensSpecification
                    new MetadataKey(MetadataSection.Exif, 42035), // LensMake
                    new MetadataKey(MetadataSection.Exif, 42036), // LensModel
                    new MetadataKey(MetadataSection.Exif, 42037), // LensSerialNumber
                    new MetadataKey(MetadataSection.Interop, 1), // InteroperabilityIndex
                    new MetadataKey(MetadataSection.Interop, 2), // InteroperabilityVersion
                    new MetadataKey(MetadataSection.Interop, 4096), // RelatedImageFileFormat
                    new MetadataKey(MetadataSection.Interop, 4097), // RelatedImageWidth
                    new MetadataKey(MetadataSection.Interop, 4098), // RelatedImageLength
                    new MetadataKey(MetadataSection.Gps, 0), // GPSVersionID
                    new MetadataKey(MetadataSection.Gps, 1), // GPSLatitudeRef
                    new MetadataKey(MetadataSection.Gps, 2), // GPSLatitude
                    new MetadataKey(MetadataSection.Gps, 3), // GPSLongitudeRef
                    new MetadataKey(MetadataSection.Gps, 4), // GPSLongitude
                    new MetadataKey(MetadataSection.Gps, 5), // GPSAltitudeRef
                    new MetadataKey(MetadataSection.Gps, 6), // GPSAltitude
                    new MetadataKey(MetadataSection.Gps, 7), // GPSTimeStamp
                    new MetadataKey(MetadataSection.Gps, 8), // GPSSatellites
                    new MetadataKey(MetadataSection.Gps, 9), // GPSStatus
                    new MetadataKey(MetadataSection.Gps, 10), // GPSMeasureMode
                    new MetadataKey(MetadataSection.Gps, 11), // GPSDOP
                    new MetadataKey(MetadataSection.Gps, 12), // GPSSpeedRef
                    new MetadataKey(MetadataSection.Gps, 13), // GPSSpeed
                    new MetadataKey(MetadataSection.Gps, 14), // GPSTrackRef
                    new MetadataKey(MetadataSection.Gps, 15), // GPSTrack
                    new MetadataKey(MetadataSection.Gps, 16), // GPSImgDirectionRef
                    new MetadataKey(MetadataSection.Gps, 17), // GPSImgDirection
                    new MetadataKey(MetadataSection.Gps, 18), // GPSMapDatum
                    new MetadataKey(MetadataSection.Gps, 19), // GPSDestLatitudeRef
                    new MetadataKey(MetadataSection.Gps, 20), // GPSDestLatitude
                    new MetadataKey(MetadataSection.Gps, 21), // GPSDestLongitudeRef
                    new MetadataKey(MetadataSection.Gps, 22), // GPSDestLongitude
                    new MetadataKey(MetadataSection.Gps, 23), // GPSDestBearingRef
                    new MetadataKey(MetadataSection.Gps, 24), // GPSDestBearing
                    new MetadataKey(MetadataSection.Gps, 25), // GPSDestDistanceRef
                    new MetadataKey(MetadataSection.Gps, 26), // GPSDestDistance
                    new MetadataKey(MetadataSection.Gps, 27), // GPSProcessingMethod
                    new MetadataKey(MetadataSection.Gps, 28), // GPSAreaInformation
                    new MetadataKey(MetadataSection.Gps, 29), // GPSDateStamp
                    new MetadataKey(MetadataSection.Gps, 30), // GPSDifferential
                    new MetadataKey(MetadataSection.Gps, 31), // GPSHPositioningError
                };
        }
    }
}
