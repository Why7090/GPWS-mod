﻿using UnityEngine;
using Jundroo.SimplePlanes.ModTools;


/// <summary>
/// The service provider through which to interact with SimplePlanes
/// </summary>
public class ServiceProvider : SimplePlanesModServiceProvider
{
    /// <summary>
    /// The singleton instance.
    /// </summary>
    private static readonly ServiceProvider _instance = new ServiceProvider();

    /// <summary>
    /// Prevents a default instance of the <see cref="ServiceProvider"/> class from being created.
    /// </summary>
    private ServiceProvider ()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            RegisterMockServices();
        }
#endif
    }

    /// <summary>
    /// Gets the singleton instance of the service provider.
    /// </summary>
    /// <value>The singleton instance of the service provider.</value>
    public static ServiceProvider Instance
    {
        get
        {
            return _instance;
        }
    }
}