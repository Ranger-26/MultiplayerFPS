// Perfect Culling (C) 2021 Patrick König
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koenigz.PerfectCulling
{
    public class PerfectCullingBakerFactory
    {
        public static PerfectCullingBaker CreateBaker(PerfectCullingBakeSettings bakeSettings)
        {
            bool isNativeRendererAvailable = PerfectCullingBakerNativeWin64.IsAvailable();

#if !UNITY_EDITOR_WIN
            isNativeRendererAvailable = false;
#endif

            if (PerfectCullingSettings.Instance.useUnityForRendering || !isNativeRendererAvailable)
            {
                return new PerfectCullingBakerUnity(bakeSettings);
            }

            return new PerfectCullingBakerNativeWin64(bakeSettings);
        }
    }
}