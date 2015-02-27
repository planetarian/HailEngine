using System;
using System.Collections.Generic;
using Artemis;
using Artemis.Attributes;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;

namespace Hail.Components
{
    [ArtemisComponentPool(InitialSize = 1000, IsResizable = true, ResizeSize = 1000, IsSupportMultiThread = true)]
    public class InputComponent : HailComponent
    {
    }
}