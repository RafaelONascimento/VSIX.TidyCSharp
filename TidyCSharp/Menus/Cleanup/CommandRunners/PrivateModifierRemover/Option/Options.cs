﻿using Geeks.VSIX.TidyCSharp.Cleanup.Infra;

namespace Geeks.VSIX.TidyCSharp.Cleanup.RemovePrivateModifier
{
    public class Options : OptionsBase, ICleanupOption
    {
        public CleanupTypes? CleanupItems => (CleanupTypes?)CleanupItemsInteger;

        public override CodeCleanerType GetCodeCleanerType()
        {
            return CodeCleanerType.PrivateAccessModifier;
        }
    }

}
