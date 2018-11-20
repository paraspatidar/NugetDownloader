using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NugetWorker
{
    public class PackageWrapper
    {
        public string packageName { get; set; }
        public NuGetVersion version { get; set; }
        public PackageIdentity rootPackageIdentity { get; set; }
        public List<PackageIdentity> childPackageIdentities { get; set; }

        public SourceRepository sourceRepository { get; set; }
        public string PossibleFolder { get
            { return $"{packageName}.{version.Version.ToString()}" ;  }
    }
        public class SamePackageAndVersion : IEqualityComparer<PackageWrapper>
        {
            //public new bool Equals(object first, object second)
            //{
            //    PackageWrapper P1 = (PackageWrapper)first;
            //    PackageWrapper P2 = (PackageWrapper)second;
            //    if ((P1.packageName == P2.packageName) && (P1.version == P2.version))
            //        return true;
            //   else
            //        return false;
            //}

            public bool Equals(PackageWrapper P1, PackageWrapper P2)
            {
                if ((P1.packageName == P2.packageName) && (P1.version == P2.version))
                    return true;
                else
                    return false;
            }

            //public int GetHashCode(object obj)
            //{
            //    PackageWrapper P0 = (PackageWrapper)obj;
            //    return P0.packageName.GetHashCode() + P0.version.GetHashCode();
            //}

            public int GetHashCode(PackageWrapper P0)
            {
                return P0.packageName.GetHashCode() + P0.version.GetHashCode();
            }

            //int IComparer.Compare(object first, object second)
            //{
            //    PackageWrapper P1 = (PackageWrapper)first;
            //    PackageWrapper P2 = (PackageWrapper)second;
            //    if ((P1.packageName== P2.packageName) && (P1.version>P2.version))
            //        return 1;
            //    if ((P1.packageName == P2.packageName) && (P1.version < P2.version))
            //        return -1;
            //    else
            //        return 0;
            //}
        }
    }
}
