using System;
using System.Collections.Generic;

namespace PlanningPoker2018_backend_2.Fleck.Helpers
{
  public static class MonoHelper
  {
    private static readonly List<int> UnixPlatformIds = new List<int>{4, 6, 128};
    
    public static bool IsRunningOnMono ()
    {
      var platformId = (int) Environment.OSVersion.Platform;
      return Type.GetType ("Mono.Runtime") != null || UnixPlatformIds.Contains(platformId);
    }
  }
}

