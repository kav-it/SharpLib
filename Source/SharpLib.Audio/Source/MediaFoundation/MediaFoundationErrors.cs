namespace NAudio.MediaFoundation
{
    internal static class MediaFoundationErrors
    {
        #region Константы

        public const int MF_E_ALLOCATOR_ALREADY_COMMITED = unchecked((int)0xC00DA7FA);

        public const int MF_E_ALLOCATOR_NOT_COMMITED = unchecked((int)0xC00DA7F9);

        public const int MF_E_ALLOCATOR_NOT_INITIALIZED = unchecked((int)0xC00DA7F8);

        public const int MF_E_ALL_PROCESS_RESTART_REQUIRED = unchecked((int)0xC00D716C);

        public const int MF_E_ALREADY_INITIALIZED = unchecked((int)0xC00D4650);

        public const int MF_E_ASF_DROPPED_PACKET = unchecked((int)0xC00D3AA1);

        public const int MF_E_ASF_FILESINK_BITRATE_UNKNOWN = unchecked((int)0xC00D4A40);

        public const int MF_E_ASF_INDEXNOTLOADED = unchecked((int)0xC00D3A9E);

        public const int MF_E_ASF_INVALIDDATA = unchecked((int)0xC00D3A9A);

        public const int MF_E_ASF_MISSINGDATA = unchecked((int)0xC00D3A99);

        public const int MF_E_ASF_NOINDEX = unchecked((int)0xC00D3A9C);

        public const int MF_E_ASF_OPAQUEPACKET = unchecked((int)0xC00D3A9B);

        public const int MF_E_ASF_OUTOFRANGE = unchecked((int)0xC00D3A9D);

        public const int MF_E_ASF_PARSINGINCOMPLETE = unchecked((int)0xC00D3A98);

        public const int MF_E_ASF_TOO_MANY_PAYLOADS = unchecked((int)0xC00D3A9F);

        public const int MF_E_ASF_UNSUPPORTED_STREAM_TYPE = unchecked((int)0xC00D3AA0);

        public const int MF_E_ATTRIBUTENOTFOUND = unchecked((int)0xC00D36E6);

        public const int MF_E_AUDIO_PLAYBACK_DEVICE_INVALIDATED = unchecked((int)0xC00D4E86);

        public const int MF_E_AUDIO_PLAYBACK_DEVICE_IN_USE = unchecked((int)0xC00D4E85);

        public const int MF_E_AUDIO_SERVICE_NOT_RUNNING = unchecked((int)0xC00D4E87);

        public const int MF_E_BACKUP_RESTRICTED_LICENSE = unchecked((int)0xC00D714E);

        public const int MF_E_BAD_OPL_STRUCTURE_FORMAT = unchecked((int)0xC00D717D);

        public const int MF_E_BAD_STARTUP_VERSION = unchecked((int)0xC00D36E3);

        public const int MF_E_BANDWIDTH_OVERRUN = unchecked((int)0xC00D4651);

        public const int MF_E_BUFFERTOOSMALL = unchecked((int)0xC00D36B1);

        public const int MF_E_BYTESTREAM_NOT_SEEKABLE = unchecked((int)0xC00D36EE);

        public const int MF_E_BYTESTREAM_UNKNOWN_LENGTH = unchecked((int)0xC00D36FB);

        public const int MF_E_CANNOT_CREATE_SINK = unchecked((int)0xC00D36FA);

        public const int MF_E_CANNOT_FIND_KEYFRAME_SAMPLE = unchecked((int)0xC00D3E9D);

        public const int MF_E_CANNOT_INDEX_IN_PLACE = unchecked((int)0xC00D4657);

        public const int MF_E_CANNOT_PARSE_BYTESTREAM = unchecked((int)0xC00D36F0);

        public const int MF_E_CLOCK_INVALID_CONTINUITY_KEY = unchecked((int)0xC00D9C40);

        public const int MF_E_CLOCK_NOT_SIMPLE = unchecked((int)0xC00D9C43);

        public const int MF_E_CLOCK_NO_TIME_SOURCE = unchecked((int)0xC00D9C41);

        public const int MF_E_CLOCK_STATE_ALREADY_SET = unchecked((int)0xC00D9C42);

        public const int MF_E_CODE_EXPIRED = unchecked((int)0xC00D715E);

        public const int MF_E_COMPONENT_REVOKED = unchecked((int)0xC00D7151);

        public const int MF_E_DEBUGGING_NOT_ALLOWED = unchecked((int)0xC00D715D);

        public const int MF_E_DISABLED_IN_SAFEMODE = unchecked((int)0xC00D36EF);

        public const int MF_E_DRM_HARDWARE_INCONSISTENT = unchecked((int)0xC00D714B);

        public const int MF_E_DRM_UNSUPPORTED = unchecked((int)0xC00D3700);

        public const int MF_E_DROPTIME_NOT_SUPPORTED = unchecked((int)0xC00DA02A);

        public const int MF_E_END_OF_STREAM = unchecked((int)0xC00D3E84);

        public const int MF_E_FLUSH_NEEDED = unchecked((int)0xC00D4653);

        public const int MF_E_FORMAT_CHANGE_NOT_SUPPORTED = unchecked((int)0xC00D36FE);

        public const int MF_E_GRL_ABSENT = unchecked((int)0xC00D7172);

        public const int MF_E_GRL_EXTENSIBLE_ENTRY_NOT_FOUND = unchecked((int)0xC00D7161);

        public const int MF_E_GRL_INVALID_FORMAT = unchecked((int)0xC00D716A);

        public const int MF_E_GRL_RENEWAL_NOT_FOUND = unchecked((int)0xC00D7160);

        public const int MF_E_GRL_UNRECOGNIZED_FORMAT = unchecked((int)0xC00D716B);

        public const int MF_E_GRL_VERSION_TOO_LOW = unchecked((int)0xC00D715F);

        public const int MF_E_HIGH_SECURITY_LEVEL_CONTENT_NOT_ALLOWED = unchecked((int)0xC00D7178);

        public const int MF_E_HW_MFT_FAILED_START_STREAMING = unchecked((int)0xC00D3704);

        public const int MF_E_HW_STREAM_NOT_CONNECTED = unchecked((int)0xC00DA7FD);

        public const int MF_E_INCOMPATIBLE_SAMPLE_PROTECTION = unchecked((int)0xC00D7176);

        public const int MF_E_INDEX_NOT_COMMITTED = unchecked((int)0xC00D4655);

        public const int MF_E_INSUFFICIENT_BUFFER = unchecked((int)0xC00D7170);

        public const int MF_E_INVALIDINDEX = unchecked((int)0xC00D36BF);

        public const int MF_E_INVALIDMEDIATYPE = unchecked((int)0xC00D36B4);

        public const int MF_E_INVALIDNAME = unchecked((int)0xC00D36BC);

        public const int MF_E_INVALIDREQUEST = unchecked((int)0xC00D36B2);

        public const int MF_E_INVALIDSTREAMNUMBER = unchecked((int)0xC00D36B3);

        public const int MF_E_INVALIDTYPE = unchecked((int)0xC00D36BD);

        public const int MF_E_INVALID_ASF_STREAMID = unchecked((int)0xC00D4659);

        public const int MF_E_INVALID_CODEC_MERIT = unchecked((int)0xC00D3703);

        public const int MF_E_INVALID_FILE_FORMAT = unchecked((int)0xC00D36BE);

        public const int MF_E_INVALID_FORMAT = unchecked((int)0xC00D3E8C);

        public const int MF_E_INVALID_KEY = unchecked((int)0xC00D36E2);

        public const int MF_E_INVALID_POSITION = unchecked((int)0xC00D36E5);

        public const int MF_E_INVALID_PROFILE = unchecked((int)0xC00D4654);

        public const int MF_E_INVALID_STATE_TRANSITION = unchecked((int)0xC00D3E82);

        public const int MF_E_INVALID_STREAM_DATA = unchecked((int)0xC00D36CB);

        public const int MF_E_INVALID_STREAM_STATE = unchecked((int)0xC00DA7FC);

        public const int MF_E_INVALID_TIMESTAMP = unchecked((int)0xC00D36C0);

        public const int MF_E_INVALID_WORKQUEUE = unchecked((int)0xC00D36FF);

        public const int MF_E_ITA_ERROR_PARSING_SAP_PARAMETERS = unchecked((int)0xC00D717B);

        public const int MF_E_ITA_OPL_DATA_NOT_INITIALIZED = unchecked((int)0xC00D7180);

        public const int MF_E_ITA_UNRECOGNIZED_ANALOG_VIDEO_OUTPUT = unchecked((int)0xC00D7181);

        public const int MF_E_ITA_UNRECOGNIZED_ANALOG_VIDEO_PROTECTION_GUID = unchecked((int)0xC00D717E);

        public const int MF_E_ITA_UNRECOGNIZED_DIGITAL_VIDEO_OUTPUT = unchecked((int)0xC00D7182);

        public const int MF_E_ITA_UNSUPPORTED_ACTION = unchecked((int)0xC00D717A);

        public const int MF_E_KERNEL_UNTRUSTED = unchecked((int)0xC00D7162);

        public const int MF_E_LATE_SAMPLE = unchecked((int)0xC00D4652);

        public const int MF_E_LICENSE_INCORRECT_RIGHTS = unchecked((int)0xC00D7148);

        public const int MF_E_LICENSE_OUTOFDATE = unchecked((int)0xC00D7149);

        public const int MF_E_LICENSE_REQUIRED = unchecked((int)0xC00D714A);

        public const int MF_E_LICENSE_RESTORE_NEEDS_INDIVIDUALIZATION = unchecked((int)0xC00D714F);

        public const int MF_E_LICENSE_RESTORE_NO_RIGHTS = unchecked((int)0xC00D714D);

        public const int MF_E_MEDIAPROC_WRONGSTATE = unchecked((int)0xC00D36F2);

        public const int MF_E_MEDIA_SOURCE_NOT_STARTED = unchecked((int)0xC00D3E91);

        public const int MF_E_MEDIA_SOURCE_NO_STREAMS_SELECTED = unchecked((int)0xC00D3E9C);

        public const int MF_E_MEDIA_SOURCE_WRONGSTATE = unchecked((int)0xC00D3E9B);

        public const int MF_E_METADATA_TOO_LONG = unchecked((int)0xC00D4A43);

        public const int MF_E_MISSING_ASF_LEAKYBUCKET = unchecked((int)0xC00D4658);

        public const int MF_E_MP3_BAD_CRC = unchecked((int)0xC00D3E99);

        public const int MF_E_MP3_NOTFOUND = unchecked((int)0xC00D3E86);

        public const int MF_E_MP3_NOTMP3 = unchecked((int)0xC00D3E88);

        public const int MF_E_MP3_NOTSUPPORTED = unchecked((int)0xC00D3E89);

        public const int MF_E_MP3_OUTOFDATA = unchecked((int)0xC00D3E87);

        public const int MF_E_MULTIPLE_BEGIN = unchecked((int)0xC00D36D9);

        public const int MF_E_MULTIPLE_SUBSCRIBERS = unchecked((int)0xC00D36DA);

        public const int MF_E_NETWORK_RESOURCE_FAILURE = unchecked((int)0xC00D4268);

        public const int MF_E_NET_BAD_CONTROL_DATA = unchecked((int)0xC00D427A);

        public const int MF_E_NET_BAD_REQUEST = unchecked((int)0xC00D427F);

        public const int MF_E_NET_BUSY = unchecked((int)0xC00D428A);

        public const int MF_E_NET_BWLEVEL_NOT_SUPPORTED = unchecked((int)0xC00D426D);

        public const int MF_E_NET_CACHESTREAM_NOT_FOUND = unchecked((int)0xC00D4271);

        public const int MF_E_NET_CACHE_NO_DATA = unchecked((int)0xC00D427D);

        public const int MF_E_NET_CANNOTCONNECT = unchecked((int)0xC00D4287);

        public const int MF_E_NET_CLIENT_CLOSE = unchecked((int)0xC00D4279);

        public const int MF_E_NET_CONNECTION_FAILURE = unchecked((int)0xC00D4283);

        public const int MF_E_NET_EOL = unchecked((int)0xC00D427E);

        public const int MF_E_NET_ERROR_FROM_PROXY = unchecked((int)0xC00D428C);

        public const int MF_E_NET_INCOMPATIBLE_PUSHSERVER = unchecked((int)0xC00D4284);

        public const int MF_E_NET_INCOMPATIBLE_SERVER = unchecked((int)0xC00D427B);

        public const int MF_E_NET_INTERNAL_SERVER_ERROR = unchecked((int)0xC00D4280);

        public const int MF_E_NET_INVALID_PRESENTATION_DESCRIPTOR = unchecked((int)0xC00D4270);

        public const int MF_E_NET_INVALID_PUSH_PUBLISHING_POINT = unchecked((int)0xC00D4289);

        public const int MF_E_NET_INVALID_PUSH_TEMPLATE = unchecked((int)0xC00D4288);

        public const int MF_E_NET_MANUALSS_NOT_SUPPORTED = unchecked((int)0xC00D426F);

        public const int MF_E_NET_NOCONNECTION = unchecked((int)0xC00D4282);

        public const int MF_E_NET_PROTOCOL_DISABLED = unchecked((int)0xC00D4294);

        public const int MF_E_NET_PROXY_ACCESSDENIED = unchecked((int)0xC00D4286);

        public const int MF_E_NET_PROXY_TIMEOUT = unchecked((int)0xC00D428D);

        public const int MF_E_NET_READ = unchecked((int)0xC00D426A);

        public const int MF_E_NET_REDIRECT = unchecked((int)0xC00D4275);

        public const int MF_E_NET_REDIRECT_TO_PROXY = unchecked((int)0xC00D4276);

        public const int MF_E_NET_REQUIRE_ASYNC = unchecked((int)0xC00D426C);

        public const int MF_E_NET_REQUIRE_INPUT = unchecked((int)0xC00D4274);

        public const int MF_E_NET_REQUIRE_NETWORK = unchecked((int)0xC00D426B);

        public const int MF_E_NET_RESOURCE_GONE = unchecked((int)0xC00D428B);

        public const int MF_E_NET_SERVER_ACCESSDENIED = unchecked((int)0xC00D4285);

        public const int MF_E_NET_SERVER_UNAVAILABLE = unchecked((int)0xC00D428E);

        public const int MF_E_NET_SESSION_INVALID = unchecked((int)0xC00D4290);

        public const int MF_E_NET_SESSION_NOT_FOUND = unchecked((int)0xC00D4281);

        public const int MF_E_NET_STREAMGROUPS_NOT_SUPPORTED = unchecked((int)0xC00D426E);

        public const int MF_E_NET_TIMEOUT = unchecked((int)0xC00D4278);

        public const int MF_E_NET_TOO_MANY_REDIRECTS = unchecked((int)0xC00D4277);

        public const int MF_E_NET_TOO_MUCH_DATA = unchecked((int)0xC00D428F);

        public const int MF_E_NET_UDP_BLOCKED = unchecked((int)0xC00D4292);

        public const int MF_E_NET_UNSAFE_URL = unchecked((int)0xC00D427C);

        public const int MF_E_NET_UNSUPPORTED_CONFIGURATION = unchecked((int)0xC00D4293);

        public const int MF_E_NET_WRITE = unchecked((int)0xC00D4269);

        public const int MF_E_NEW_VIDEO_DEVICE = unchecked((int)0xC00D4E25);

        public const int MF_E_NON_PE_PROCESS = unchecked((int)0xC00D7165);

        public const int MF_E_NOTACCEPTING = unchecked((int)0xC00D36B5);

        public const int MF_E_NOT_AVAILABLE = unchecked((int)0xC00D36D6);

        public const int MF_E_NOT_FOUND = unchecked((int)0xC00D36D5);

        public const int MF_E_NOT_INITIALIZED = unchecked((int)0xC00D36B6);

        public const int MF_E_NOT_PROTECTED = unchecked((int)0xC00D3E9A);

        public const int MF_E_NO_AUDIO_PLAYBACK_DEVICE = unchecked((int)0xC00D4E84);

        public const int MF_E_NO_BITPUMP = unchecked((int)0xC00D36F6);

        public const int MF_E_NO_CLOCK = unchecked((int)0xC00D36D7);

        public const int MF_E_NO_CONTENT_PROTECTION_MANAGER = unchecked((int)0xC00D714C);

        public const int MF_E_NO_DURATION = unchecked((int)0xC00D3E8A);

        public const int MF_E_NO_EVENTS_AVAILABLE = unchecked((int)0xC00D3E80);

        public const int MF_E_NO_INDEX = unchecked((int)0xC00D4656);

        public const int MF_E_NO_MORE_DROP_MODES = unchecked((int)0xC00DA028);

        public const int MF_E_NO_MORE_QUALITY_LEVELS = unchecked((int)0xC00DA029);

        public const int MF_E_NO_MORE_TYPES = unchecked((int)0xC00D36B9);

        public const int MF_E_NO_PMP_HOST = unchecked((int)0xC00D717F);

        public const int MF_E_NO_SAMPLE_DURATION = unchecked((int)0xC00D36C9);

        public const int MF_E_NO_SAMPLE_TIMESTAMP = unchecked((int)0xC00D36C8);

        public const int MF_E_NO_SOURCE_IN_CACHE = unchecked((int)0xC00D61AE);

        public const int MF_E_NO_VIDEO_SAMPLE_AVAILABLE = unchecked((int)0xC00D4E26);

        public const int MF_E_OFFLINE_MODE = unchecked((int)0xC00D4291);

        public const int MF_E_OPERATION_CANCELLED = unchecked((int)0xC00D36ED);

        public const int MF_E_OPL_NOT_SUPPORTED = unchecked((int)0xC00D715A);

        public const int MF_E_OUT_OF_RANGE = unchecked((int)0xC00D3702);

        public const int MF_E_PEAUTH_NOT_STARTED = unchecked((int)0xC00D7175);

        public const int MF_E_PEAUTH_PUBLICKEY_REVOKED = unchecked((int)0xC00D7171);

        public const int MF_E_PEAUTH_SESSION_NOT_STARTED = unchecked((int)0xC00D716F);

        public const int MF_E_PEAUTH_UNTRUSTED = unchecked((int)0xC00D7163);

        public const int MF_E_PE_SESSIONS_MAXED = unchecked((int)0xC00D7177);

        public const int MF_E_PE_UNTRUSTED = unchecked((int)0xC00D7174);

        public const int MF_E_PLATFORM_NOT_INITIALIZED = unchecked((int)0xC00D36B0);

        public const int MF_E_POLICY_MGR_ACTION_OUTOFBOUNDS = unchecked((int)0xC00D717C);

        public const int MF_E_POLICY_UNSUPPORTED = unchecked((int)0xC00D7159);

        public const int MF_E_PROCESS_RESTART_REQUIRED = unchecked((int)0xC00D716D);

        public const int MF_E_PROPERTY_EMPTY = unchecked((int)0xC00D36E9);

        public const int MF_E_PROPERTY_NOT_ALLOWED = unchecked((int)0xC00D3E8F);

        public const int MF_E_PROPERTY_NOT_EMPTY = unchecked((int)0xC00D36EA);

        public const int MF_E_PROPERTY_NOT_FOUND = unchecked((int)0xC00D3E8D);

        public const int MF_E_PROPERTY_READ_ONLY = unchecked((int)0xC00D3E8E);

        public const int MF_E_PROPERTY_TYPE_NOT_ALLOWED = unchecked((int)0xC00D36E7);

        public const int MF_E_PROPERTY_TYPE_NOT_SUPPORTED = unchecked((int)0xC00D36E8);

        public const int MF_E_PROPERTY_VECTOR_NOT_ALLOWED = unchecked((int)0xC00D36EB);

        public const int MF_E_PROPERTY_VECTOR_REQUIRED = unchecked((int)0xC00D36EC);

        public const int MF_E_QM_INVALIDSTATE = unchecked((int)0xC00DA02C);

        public const int MF_E_QUALITYKNOB_WAIT_LONGER = unchecked((int)0xC00DA02B);

        public const int MF_E_RATE_CHANGE_PREEMPTED = unchecked((int)0xC00D36D4);

        public const int MF_E_REBOOT_REQUIRED = unchecked((int)0xC00D7167);

        public const int MF_E_REVERSE_UNSUPPORTED = unchecked((int)0xC00D36D2);

        public const int MF_E_RT_OUTOFMEMORY = unchecked((int)0xC00D36F7);

        public const int MF_E_RT_THROUGHPUT_NOT_AVAILABLE = unchecked((int)0xC00D36F3);

        public const int MF_E_RT_TOO_MANY_CLASSES = unchecked((int)0xC00D36F4);

        public const int MF_E_RT_UNAVAILABLE = unchecked((int)0xC00D36CF);

        public const int MF_E_RT_WORKQUEUE_CLASS_NOT_SPECIFIED = unchecked((int)0xC00D36F8);

        public const int MF_E_RT_WOULDBLOCK = unchecked((int)0xC00D36F5);

        public const int MF_E_SAMPLEALLOCATOR_CANCELED = unchecked((int)0xC00D4A3D);

        public const int MF_E_SAMPLEALLOCATOR_EMPTY = unchecked((int)0xC00D4A3E);

        public const int MF_E_SAMPLE_HAS_TOO_MANY_BUFFERS = unchecked((int)0xC00D36DF);

        public const int MF_E_SAMPLE_NOT_WRITABLE = unchecked((int)0xC00D36E0);

        public const int MF_E_SEQUENCER_UNKNOWN_SEGMENT_ID = unchecked((int)0xC00D61AC);

        public const int MF_E_SESSION_PAUSEWHILESTOPPED = unchecked((int)0xC00D36FC);

        public const int MF_E_SHUTDOWN = unchecked((int)0xC00D3E85);

        public const int MF_E_SIGNATURE_VERIFICATION_FAILED = unchecked((int)0xC00D715C);

        public const int MF_E_SINK_ALREADYSTOPPED = unchecked((int)0xC00D4A3F);

        public const int MF_E_SINK_NO_SAMPLES_PROCESSED = unchecked((int)0xC00D4A44);

        public const int MF_E_SINK_NO_STREAMS = unchecked((int)0xC00D4A41);

        public const int MF_E_SOURCERESOLVER_MUTUALLY_EXCLUSIVE_FLAGS = unchecked((int)0xC00D36F1);

        public const int MF_E_STATE_TRANSITION_PENDING = unchecked((int)0xC00D36DC);

        public const int MF_E_STREAMSINKS_FIXED = unchecked((int)0xC00D4A3B);

        public const int MF_E_STREAMSINKS_OUT_OF_SYNC = unchecked((int)0xC00D4A3A);

        public const int MF_E_STREAMSINK_EXISTS = unchecked((int)0xC00D4A3C);

        public const int MF_E_STREAMSINK_REMOVED = unchecked((int)0xC00D4A38);

        public const int MF_E_STREAM_ERROR = unchecked((int)0xC00DA7FB);

        public const int MF_E_TEST_SIGNED_COMPONENTS_NOT_ALLOWED = unchecked((int)0xC00D7179);

        public const int MF_E_THINNING_UNSUPPORTED = unchecked((int)0xC00D36D1);

        public const int MF_E_TIMER_ORPHANED = unchecked((int)0xC00D36DB);

        public const int MF_E_TOPOLOGY_VERIFICATION_FAILED = unchecked((int)0xC00D715B);

        public const int MF_E_TOPO_CANNOT_CONNECT = unchecked((int)0xC00D5213);

        public const int MF_E_TOPO_CANNOT_FIND_DECRYPTOR = unchecked((int)0xC00D5211);

        public const int MF_E_TOPO_CODEC_NOT_FOUND = unchecked((int)0xC00D5212);

        public const int MF_E_TOPO_INVALID_OPTIONAL_NODE = unchecked((int)0xC00D520E);

        public const int MF_E_TOPO_INVALID_TIME_ATTRIBUTES = unchecked((int)0xC00D5215);

        public const int MF_E_TOPO_LOOPS_IN_TOPOLOGY = unchecked((int)0xC00D5216);

        public const int MF_E_TOPO_MISSING_PRESENTATION_DESCRIPTOR = unchecked((int)0xC00D5217);

        public const int MF_E_TOPO_MISSING_SOURCE = unchecked((int)0xC00D521A);

        public const int MF_E_TOPO_MISSING_STREAM_DESCRIPTOR = unchecked((int)0xC00D5218);

        public const int MF_E_TOPO_SINK_ACTIVATES_UNSUPPORTED = unchecked((int)0xC00D521B);

        public const int MF_E_TOPO_STREAM_DESCRIPTOR_NOT_SELECTED = unchecked((int)0xC00D5219);

        public const int MF_E_TOPO_UNSUPPORTED = unchecked((int)0xC00D5214);

        public const int MF_E_TRANSCODE_NO_CONTAINERTYPE = unchecked((int)0xC00DA410);

        public const int MF_E_TRANSCODE_NO_MATCHING_ENCODER = unchecked((int)0xC00DA412);

        public const int MF_E_TRANSCODE_PROFILE_NO_MATCHING_STREAMS = unchecked((int)0xC00DA411);

        public const int MF_E_TRANSFORM_ASYNC_LOCKED = unchecked((int)0xC00D6D77);

        public const int MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING = unchecked((int)0xC00D6D74);

        public const int MF_E_TRANSFORM_CANNOT_INITIALIZE_ACM_DRIVER = unchecked((int)0xC00D6D78);

        public const int MF_E_TRANSFORM_CONFLICTS_WITH_OTHER_CURRENTLY_ENABLED_FEATURES = unchecked((int)0xC00D6D70);

        public const int MF_E_TRANSFORM_INPUT_REMAINING = unchecked((int)0xC00D6D62);

        public const int MF_E_TRANSFORM_NEED_MORE_INPUT = unchecked((int)0xC00D6D72);

        public const int MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_INPUT_MEDIATYPE = unchecked((int)0xC00D6D6E);

        public const int MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_MEDIATYPE_COMBINATION = unchecked((int)0xC00D6D6F);

        public const int MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_OUTPUT_MEDIATYPE = unchecked((int)0xC00D6D6D);

        public const int MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_SPKR_CONFIG = unchecked((int)0xC00D6D73);

        public const int MF_E_TRANSFORM_PROFILE_INVALID_OR_CORRUPT = unchecked((int)0xC00D6D64);

        public const int MF_E_TRANSFORM_PROFILE_MISSING = unchecked((int)0xC00D6D63);

        public const int MF_E_TRANSFORM_PROFILE_TRUNCATED = unchecked((int)0xC00D6D65);

        public const int MF_E_TRANSFORM_PROPERTY_ARRAY_VALUE_WRONG_NUM_DIM = unchecked((int)0xC00D6D69);

        public const int MF_E_TRANSFORM_PROPERTY_NOT_WRITEABLE = unchecked((int)0xC00D6D68);

        public const int MF_E_TRANSFORM_PROPERTY_PID_NOT_RECOGNIZED = unchecked((int)0xC00D6D66);

        public const int MF_E_TRANSFORM_PROPERTY_VALUE_INCOMPATIBLE = unchecked((int)0xC00D6D6C);

        public const int MF_E_TRANSFORM_PROPERTY_VALUE_OUT_OF_RANGE = unchecked((int)0xC00D6D6B);

        public const int MF_E_TRANSFORM_PROPERTY_VALUE_SIZE_WRONG = unchecked((int)0xC00D6D6A);

        public const int MF_E_TRANSFORM_PROPERTY_VARIANT_TYPE_WRONG = unchecked((int)0xC00D6D67);

        public const int MF_E_TRANSFORM_STREAM_CHANGE = unchecked((int)0xC00D6D61);

        public const int MF_E_TRANSFORM_TYPE_NOT_SET = unchecked((int)0xC00D6D60);

        public const int MF_E_TRUST_DISABLED = unchecked((int)0xC00D7152);

        public const int MF_E_UNAUTHORIZED = unchecked((int)0xC00D3701);

        public const int MF_E_UNEXPECTED = unchecked((int)0xC00D36BB);

        public const int MF_E_UNRECOVERABLE_ERROR_OCCURRED = unchecked((int)0xC00D36DE);

        public const int MF_E_UNSUPPORTED_BYTESTREAM_TYPE = unchecked((int)0xC00D36C4);

        public const int MF_E_UNSUPPORTED_CAPTION = unchecked((int)0xC00D36E4);

        public const int MF_E_UNSUPPORTED_D3D_TYPE = unchecked((int)0xC00D6D76);

        public const int MF_E_UNSUPPORTED_FORMAT = unchecked((int)0xC00D3E98);

        public const int MF_E_UNSUPPORTED_RATE = unchecked((int)0xC00D36D0);

        public const int MF_E_UNSUPPORTED_RATE_TRANSITION = unchecked((int)0xC00D36D3);

        public const int MF_E_UNSUPPORTED_REPRESENTATION = unchecked((int)0xC00D36B7);

        public const int MF_E_UNSUPPORTED_SCHEME = unchecked((int)0xC00D36C3);

        public const int MF_E_UNSUPPORTED_SERVICE = unchecked((int)0xC00D36BA);

        public const int MF_E_UNSUPPORTED_STATE_TRANSITION = unchecked((int)0xC00D36DD);

        public const int MF_E_UNSUPPORTED_TIME_FORMAT = unchecked((int)0xC00D36C5);

        public const int MF_E_USERMODE_UNTRUSTED = unchecked((int)0xC00D716E);

        public const int MF_E_VIDEO_DEVICE_LOCKED = unchecked((int)0xC00D4E24);

        public const int MF_E_VIDEO_REN_COPYPROT_FAILED = unchecked((int)0xC00D4E22);

        public const int MF_E_VIDEO_REN_NO_DEINTERLACE_HW = unchecked((int)0xC00D4E21);

        public const int MF_E_VIDEO_REN_NO_PROCAMP_HW = unchecked((int)0xC00D4E20);

        public const int MF_E_VIDEO_REN_SURFACE_NOT_SHARED = unchecked((int)0xC00D4E23);

        public const int MF_E_WMDRMOTA_ACTION_ALREADY_SET = unchecked((int)0xC00D7154);

        public const int MF_E_WMDRMOTA_ACTION_MISMATCH = unchecked((int)0xC00D7157);

        public const int MF_E_WMDRMOTA_DRM_ENCRYPTION_SCHEME_NOT_SUPPORTED = unchecked((int)0xC00D7156);

        public const int MF_E_WMDRMOTA_DRM_HEADER_NOT_AVAILABLE = unchecked((int)0xC00D7155);

        public const int MF_E_WMDRMOTA_INVALID_POLICY = unchecked((int)0xC00D7158);

        public const int MF_E_WMDRMOTA_NO_ACTION = unchecked((int)0xC00D7153);

        public const int MF_I_MANUAL_PROXY = unchecked(0x400D4272);

        public const int MF_S_ACTIVATE_REPLACED = unchecked(0x000D36FD);

        public const int MF_S_ASF_PARSEINPROGRESS = unchecked(0x400D3A98);

        public const int MF_S_CLOCK_STOPPED = unchecked(0x000D9C44);

        public const int MF_S_MULTIPLE_BEGIN = unchecked(0x000D36D8);

        public const int MF_S_PE_TRUSTED = unchecked(0x000D7173);

        public const int MF_S_PROTECTION_NOT_REQUIRED = unchecked(0x000D7150);

        public const int MF_S_SEQUENCER_CONTEXT_CANCELED = unchecked(0x000D61AD);

        public const int MF_S_SEQUENCER_SEGMENT_AT_END_OF_STREAM = unchecked(0x000D61AF);

        public const int MF_S_SINK_NOT_FINALIZED = unchecked(0x000D4A42);

        public const int MF_S_TRANSFORM_DO_NOT_PROPAGATE_EVENT = unchecked(0x000D6D75);

        public const int MF_S_VIDEO_DISABLED_WITH_UNKNOWN_SOFTWARE_OUTPUT = unchecked(0x000D7169);

        public const int MF_S_WAIT_FOR_POLICY_SET = unchecked(0x000D7168);

        #endregion
    }
}