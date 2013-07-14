﻿//
// DemeterFluentExtensions.cs
//
// Author:
//       John Moore <jcwmoore@gmail.com>
//
// Copyright (c) 2013 John Moore
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace Demeter
{
    public static class DemeterRegistationContextExtensions
    {
        /// <summary>
        /// Registers a type to its self
        /// </summary>
        public static DemeterRegistration<TInterface, TInterface> ToItsSelf<TInterface>(this DemeterRegistationContext<TInterface> rc) where TInterface : class
        {
            return rc.To<TInterface>();
        }

        /// <summary>
        /// Assigns a name to this registration
        /// </summary>
        /// <param name="name">Name for the registration</param>
        /// <returns>Original object for Fluent API</returns>
        public static DemeterRegistationContext<TInterface> Named<TInterface>(this DemeterRegistationContext<TInterface> rc, string name)
        {
            rc.ResolutionInfo.Name = name;
            return rc;
        }
    }

    public static class DemeterRegistrationExtensions
    {
        /// <summary>
        /// Specifies this Registration as a Singleton
        /// </summary>
        public static void AsSingleton<TInterface, TImplementation>(this DemeterRegistration<TInterface, TImplementation> jr) where TImplementation : class, TInterface
        {
            jr.ResolutionInfo.LifeCycleManager = new Container.SingletonObjectLifecycle<TInterface, TImplementation>(jr.ResolutionInfo, null);
        }

        /// <summary>
        /// Specifies this Registration as a Singleton
        /// </summary>
        /// <param name="obj">Singleton object</param>
        public static void AsSingleton<TInterface, TImplementation>(this DemeterRegistration<TInterface, TImplementation> jr, TImplementation obj) where TImplementation : class, TInterface
        {
            jr.ResolutionInfo.LifeCycleManager = new Container.SingletonObjectLifecycle<TInterface, TImplementation>(jr.ResolutionInfo, obj);
        }

        /// <summary>
        /// Specifies this Registration as a Singleton
        /// </summary>
        /// <param name="obj">Singleton object</param>
        public static void AsValueSingleton<TInterface, TImplementation>(this DemeterRegistration<TInterface, TImplementation> jr, TImplementation obj) where TImplementation : struct, TInterface
        {
            jr.ResolutionInfo.LifeCycleManager = new Container.SingletonStructLifecycle<TInterface, TImplementation>(jr.ResolutionInfo, obj);
        }

        /// <summary>
        /// Specifies this Registration as a Transient
        /// </summary>
        public static void AsTransient<TInterface, TImplementation>(this DemeterRegistration<TInterface, TImplementation> jr) where TImplementation : TInterface
        {
            if (typeof(TImplementation).IsAbstract && jr.ResolutionInfo.IsReflectionBuild)
            {
                throw new RegistrationException(string.Format("Can not register {0} as transient, a defined constructor must be provided", typeof(TInterface).Name), typeof(TImplementation));
            }
            jr.ResolutionInfo.LifeCycleManager = new Container.TransientLifecycle<TInterface>(jr.ResolutionInfo);
        }
    }
}
