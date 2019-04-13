﻿/*
Copyright (c) 2013, 2014 Paolo Patierno

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution. 

The Eclipse Public License is available at 
   http://www.eclipse.org/legal/epl-v10.html
and the Eclipse Distribution License is available at 
   http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Paolo Patierno - initial API and implementation and/or initial documentation
*/

#if !(NETSTANDARD1_6 || NETSTANDARD2_0)
using System.Threading;
using System.Threading.Tasks;

namespace uPLibrary.Networking.M2Mqtt
{
    /// <summary>
    /// Support methods fos specific framework
    /// </summary>
    public class Fx
    {

        public delegate void ThreadStart();
        public static void StartThread(ThreadStart threadStart)
        {
            Task.Factory.StartNew(o => ((ThreadStart)o)(), threadStart);
        }

        public static void SleepThread(int millisecondsTimeout) { Task.Delay(millisecondsTimeout).RunSynchronously(); }
    }
}
#endif