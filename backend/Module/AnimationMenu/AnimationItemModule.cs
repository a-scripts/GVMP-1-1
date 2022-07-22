﻿using System;
using VMP_CNR.Module.Assets.Tattoo;

namespace VMP_CNR.Module.AnimationMenu
{
    public class AnimationItemModule : SqlModule<AnimationItemModule, AnimationItem, uint>
    {
        public override Type[] RequiredModules()
        {
            return new[] { typeof(AnimationCategoryModule) };
        }

        protected override string GetQuery()
        {
            return "SELECT * FROM `animationmenu_item` ORDER by `name`;";
        }
    }
}
