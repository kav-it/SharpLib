﻿namespace SharpLib.Audio.MediaFoundation
{
    internal enum MediaEventType
    {
        MEUnknown = 0,

        MEError = 1,

        MEExtendedType = 2,

        MENonFatalError = 3,

        MESessionUnknown = 100,

        MESessionTopologySet = 101,

        MESessionTopologiesCleared = 102,

        MESessionStarted = 103,

        MESessionPaused = 104,

        MESessionStopped = 105,

        MESessionClosed = 106,

        MESessionEnded = 107,

        MESessionRateChanged = 108,

        MESessionScrubSampleComplete = 109,

        MESessionCapabilitiesChanged = 110,

        MESessionTopologyStatus = 111,

        MESessionNotifyPresentationTime = 112,

        MENewPresentation = 113,

        MELicenseAcquisitionStart = 114,

        MELicenseAcquisitionCompleted = 115,

        MEIndividualizationStart = 116,

        MEIndividualizationCompleted = 117,

        MEEnablerProgress = 118,

        MEEnablerCompleted = 119,

        MEPolicyError = 120,

        MEPolicyReport = 121,

        MEBufferingStarted = 122,

        MEBufferingStopped = 123,

        MEConnectStart = 124,

        MEConnectEnd = 125,

        MEReconnectStart = 126,

        MEReconnectEnd = 127,

        MERendererEvent = 128,

        MESessionStreamSinkFormatChanged = 129,

        MESourceUnknown = 200,

        MESourceStarted = 201,

        MEStreamStarted = 202,

        MESourceSeeked = 203,

        MEStreamSeeked = 204,

        MENewStream = 205,

        MEUpdatedStream = 206,

        MESourceStopped = 207,

        MEStreamStopped = 208,

        MESourcePaused = 209,

        MEStreamPaused = 210,

        MEEndOfPresentation = 211,

        MEEndOfStream = 212,

        MEMediaSample = 213,

        MEStreamTick = 214,

        MEStreamThinMode = 215,

        MEStreamFormatChanged = 216,

        MESourceRateChanged = 217,

        MEEndOfPresentationSegment = 218,

        MESourceCharacteristicsChanged = 219,

        MESourceRateChangeRequested = 220,

        MESourceMetadataChanged = 221,

        MESequencerSourceTopologyUpdated = 222,

        MESinkUnknown = 300,

        MEStreamSinkStarted = 301,

        MEStreamSinkStopped = 302,

        MEStreamSinkPaused = 303,

        MEStreamSinkRateChanged = 304,

        MEStreamSinkRequestSample = 305,

        MEStreamSinkMarker = 306,

        MEStreamSinkPrerolled = 307,

        MEStreamSinkScrubSampleComplete = 308,

        MEStreamSinkFormatChanged = 309,

        MEStreamSinkDeviceChanged = 310,

        MEQualityNotify = 311,

        MESinkInvalidated = 312,

        MEAudioSessionNameChanged = 313,

        MEAudioSessionVolumeChanged = 314,

        MEAudioSessionDeviceRemoved = 315,

        MEAudioSessionServerShutdown = 316,

        MEAudioSessionGroupingParamChanged = 317,

        MEAudioSessionIconChanged = 318,

        MEAudioSessionFormatChanged = 319,

        MEAudioSessionDisconnected = 320,

        MEAudioSessionExclusiveModeOverride = 321,

        METrustUnknown = 400,

        MEPolicyChanged = 401,

        MEContentProtectionMessage = 402,

        MEPolicySet = 403,

        MEWMDRMLicenseBackupCompleted = 500,

        MEWMDRMLicenseBackupProgress = 501,

        MEWMDRMLicenseRestoreCompleted = 502,

        MEWMDRMLicenseRestoreProgress = 503,

        MEWMDRMLicenseAcquisitionCompleted = 506,

        MEWMDRMIndividualizationCompleted = 508,

        MEWMDRMIndividualizationProgress = 513,

        MEWMDRMProximityCompleted = 514,

        MEWMDRMLicenseStoreCleaned = 515,

        MEWMDRMRevocationDownloadCompleted = 516,

        METransformUnknown = 600,

        METransformNeedInput = (METransformUnknown + 1),

        METransformHaveOutput = (METransformNeedInput + 1),

        METransformDrainComplete = (METransformHaveOutput + 1),

        METransformMarker = (METransformDrainComplete + 1),
    }
}