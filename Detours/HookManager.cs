﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HookManager.cs" company="xcvd">
//   Copyright 2012 - xcvd
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
// 
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
// 
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
//   Manages all hooks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Detours
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Manages all hooks.
    /// </summary>
    public static class HookManager
    {
        #region Constants and Fields

        /// <summary>
        /// Gets Hooks.
        /// </summary>
        internal static readonly Dictionary<string, Hook> Hooks = new Dictionary<string, Hook>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a hook to the hook manager.
        /// </summary>
        /// <param name="target">
        ///   The target delegate method.
        /// </param>
        /// <param name="hook">
        ///   The hook delegate method.
        /// </param>
        /// <param name="name">
        ///   The name of the hook.
        /// </param>
        public static void Add(Delegate target, Delegate hook, string name)
        {
            Hooks.Add(name, new Hook(target, hook));
        }

        /// <summary>
        /// Calls the original function, and returns a return value.
        /// </summary>
        /// <param name="name">
        ///   The name. 
        /// </param>       
        /// <param name="args">
        ///   The arguments to pass. If it is a 'void' argument list,
        ///   you must pass 'null'.
        /// </param>
        /// <returns>
        /// An object containing the original functions return value.
        /// </returns>
        public static object CallOriginal(string name, params object[] args)
        {
            return Hooks[name].CallOriginal(args);
        }

        /// <summary>
        /// Clears all hooks from the hook manager.
        /// </summary>
        public static void Clear()
        {
            Hooks.Clear();            
        }

        /// <summary>
        /// Installs the hook of a given name.
        /// </summary>
        /// <param name="name">
        ///  The name of the hook to install.
        /// </param>
        /// <exception cref="HookNotFoundException">
        /// Thrown if the named hook has not yet been added.
        /// </exception>
        /// <exception cref="HookInstallFailedException">
        /// Thrown if the named hook fails to installs.
        /// </exception>
        public static void Install(string name)
        {
            if (!Hooks.ContainsKey(name))
            {
                throw new HookNotFoundException(
                    "The hook " + name + " could not be found. Verify that you have added the hook " + name + ".");
            }

            if (!Hooks[name].Install())
            {
                throw new HookInstallFailedException(
                    "The hook " + name + " failed to install. Verify addresses are correct.");
            }
        }

        /// <summary>
        /// Installs all hooks.
        /// </summary>
        /// <exception cref="HookInstallFailedException">
        /// Thrown if the certain hook fails to installs.
        /// </exception>
        public static void InstallAll()
        {
            foreach (var hook in Hooks.Where(hook => !hook.Value.Install()))
            {
                throw new HookInstallFailedException(
                    "The hook " + hook.Key + " failed to install. Verify addresses are correct.");
            }
        }

        /// <summary>
        /// Deletes a hook from the hook manager.
        /// </summary>
        /// <param name="name">
        ///   The name of the hook.
        /// </param>
        public static void Remove(string name)
        {
            if (Hooks[name].IsInstalled)
            {
                Uninstall(name);
            }

            Hooks.Remove(name);
        }

        /// <summary>
        /// Removes the hook of a given name.
        /// </summary>
        /// <param name="name">
        /// The name of the hook to uninstall.
        /// </param>
        /// <exception cref="HookNotFoundException">
        /// Thrown if the named hook has not yet been added.
        /// </exception>
        /// <exception cref="HookUninstallFailedException">
        /// Thrown if the named hook fails to uninstall.
        /// </exception>
        public static void Uninstall(string name)
        {
            if (!Hooks.ContainsKey(name))
            {
                throw new HookNotFoundException(
                    "The hook " + name + " could not be found. Verify that you have added the hook " + name + ".");
            }

            if (!Hooks[name].Uninstall())
            {
                throw new HookUninstallFailedException(
                    "The hook " + name + " failed to uninstall. Very addresses are correct.");
            }
        }

        /// <summary>
        /// Uninstalls all hooks.
        /// </summary>
        /// <exception cref="HookUninstallFailedException">
        /// Thrown if the certain hook fails to installs.
        /// </exception>
        public static void UninstallAll()
        {
            foreach (var hook in Hooks.Where(hook => !hook.Value.Uninstall()))
            {
                throw new HookUninstallFailedException(
                    "The hook " + hook.Key + " failed to uninstall. Verify addresses are correct.");
            }
        }

        #endregion
    }
}