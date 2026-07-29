// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using Aspose.Collections;
using Aspose.TestFx.Pal;

namespace Aspose.TestFx.GoldComparers.Format
{
    internal class RemoteComparer : AbstractGoldComparer
    {
        private readonly DocumentType mType;

        public RemoteComparer(DocumentType type)
        {
            mType = type;
        }

        public override void Execute(ComparerParams comparerParams)
        {
            StringToStringDictionary httpParameters = comparerParams.ToParamMap();
            httpParameters.Add("TYPE", ((int)mType).ToString());
            HttpClientPal.DoPost(gServerUrl, httpParameters);
        }
        
        public override void VerifyConformance(ComparerParams comparerParams)
        {
            StringToStringDictionary httpParameters = comparerParams.ToParamMap();
            httpParameters.Add("TYPE", ((int)mType).ToString());
            httpParameters.Add("CONF", "true");
            HttpClientPal.DoPost(gServerUrl, httpParameters);
        }

        public override bool ExecuteForm(ComparerParams comparerParams)
        {
            // Always empty
            return false;
        }

        private static readonly string gServerUrl = "http://" + TestEnvironment.GetRemoteComparerIP() + 
                                                    ":" + TestEnvironment.GetRemoteComparerPort();
    }
}
