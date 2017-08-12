﻿using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineCase.Server.Game.Entities
{
    class EntitiesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PlayerGrain>();
        }
    }
}