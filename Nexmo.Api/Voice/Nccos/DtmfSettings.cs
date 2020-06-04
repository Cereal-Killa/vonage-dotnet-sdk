﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexmo.Api.Voice.Nccos
{
    public class DtmfSettings
    {
        [JsonProperty("timeOut")]
        public int? TimeOut { get; set; }

        [JsonProperty("maxDigits")]
        public int? MaxDigits { get; set; }

        [JsonProperty("submitOnHash")]
        public bool? SubmitOnHash { get; set; }
    }
}
