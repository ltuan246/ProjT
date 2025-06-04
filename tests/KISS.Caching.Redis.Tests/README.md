# C# Caching Strategies

[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/)

A comprehensive guide to caching strategies in C#. This repository explains how to use caching to improve application performance by reducing latency and load on data sources like databases or APIs.

## 🚀 Features
- In-memory caching with `IMemoryCache`
- Distributed caching with Redis
- Lazy (Cache-Aside), Write-Through, and Write-Behind strategies
- Time-based expiration (TTL) and eviction policies
- Practical tips for scalability and performance

## 📋 Table of Contents
- [Introduction](#introduction)
- [Caching Strategies](#caching-strategies)
  - [In-Memory Caching](#in-memory-caching)
  - [Distributed Caching](#distributed-caching)
  - [Lazy Caching (Cache-Aside)](#lazy-caching-cache-aside)
  - [Write-Through Caching](#write-through-caching)
  - [Write-Behind Caching](#write-behind-caching)
  - [Time-Based Expiration (TTL)](#time-based-expiration-ttl)
  - [Eviction Policies](#eviction-policies)
- [Best Practices](#best-practices)

## 📖 Introduction
Caching stores frequently accessed data in memory to minimize latency and reduce load on slower data sources. This repo explains various caching strategies in C#, suitable for single-server apps, distributed systems, or microservices.

## 🛠️ Caching Strategies

### In-Memory Caching
Stores data in the application's memory for fast access. Best for small datasets in single-server apps.

```csharp
using Microsoft.Extensions.Caching.Memory;

public class InMemoryCacheExample
{
    private readonly IMemoryCache _cache;

    public InMemoryCacheExample(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GetCachedData(string key)
    {
        if (!_cache.TryGetValue(key, out string cachedData))
        {
            cachedData = FetchDataFromSource();
            _cache.Set(key, cachedData, TimeSpan.FromMinutes(5));
        }
        return cachedData;
    }

    private string FetchDataFromSource() => "Data from source";
}
```

**Use Case**: Session data, lookup tables.  
**Pros**: Fast, simple to implement.  
**Cons**: Limited by memory; data lost on app restart.

### Distributed Caching
Uses external systems like Redis for shared caching across multiple app instances.

```csharp
using StackExchange.Redis;

public class RedisCacheExample
{
    private readonly IDatabase _redisDb;

    public RedisCacheExample(ConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task<string> GetCachedDataAsync(string key)
    {
        var cachedData = await _redisDb.StringGetAsync(key);
        if (cachedData.IsNull)
        {
            var data = FetchDataFromSource();
            await _redisDb.StringSetAsync(key, data, TimeSpan.FromMinutes(5));
            return data;
        }
        return cachedData;
    }

    private string FetchDataFromSource() => "Data from source";
}
```

**Use Case**: Microservices, cloud apps.  
**Pros**: Scalable, persistent across restarts.  
**Cons**: Requires external infrastructure, adds network latency.

### Lazy Caching (Cache-Aside)
Loads data into the cache on-demand during cache misses, with the application managing cache population.

```csharp
using Microsoft.Extensions.Caching.Memory;

public class LazyCacheExample
{
    private readonly IMemoryCache _cache;

    public LazyCacheExample(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GetCachedData(string key)
    {
        if (!_cache.TryGetValue(key, out string cachedData))
        {
            cachedData = FetchDataFromSource();
            _cache.Set(key, cachedData, TimeSpan.FromMinutes(5));
        }
        return cachedData;
    }

    private string FetchDataFromSource() => "Data from source";
}
```

**Use Case**: Unpredictable access patterns.  
**Pros**: Efficient for sporadic data access.  
**Cons**: Cache miss overhead; requires manual cache management.

### Write-Through Caching
Writes data to both cache and data source simultaneously, ensuring consistency.

```csharp
using Microsoft.Extensions.Caching.Memory;

public class WriteThroughCacheExample
{
    private readonly IMemoryCache _cache;

    public WriteThroughCacheExample(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task UpdateDataAsync(string key, string value)
    {
        await UpdateDataSourceAsync(key, value); // Update DB
        _cache.Set(key, value, TimeSpan.FromMinutes(10));
    }

    private async Task UpdateDataSourceAsync(string key, string value)
    {
        // Simulate DB update
        await Task.Delay(100);
    }
}
```

**Use Case**: Consistency-critical apps (e.g., financial systems).  
**Pros**: Ensures data consistency.  
**Cons**: Slower writes due to dual updates.

### Write-Behind Caching
Writes data to cache first, syncing to the data source asynchronously.

```csharp
using Microsoft.Extensions.Caching.Memory;

public class WriteBehindCacheExample
{
    private readonly IMemoryCache _cache;

    public WriteBehindCacheExample(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task WriteBehindAsync(string key, string value)
    {
        _cache.Set(key, value); // Immediate cache update
        await Task.Run(() => UpdateDataSourceAsync(key, value)); // Async DB update
    }

    private async Task UpdateDataSourceAsync(string key, string value)
    {
        // Simulate DB update
        await Task.Delay(100);
    }
}
```

**Use Case**: High-throughput writes (e.g., logging).  
**Pros**: Fast writes, reduced latency.  
**Cons**: Risk of data loss if cache fails before sync.

### Time-Based Expiration (TTL)
Removes cached items after a set time (absolute or sliding expiration).

```csharp
using Microsoft.Extensions.Caching.Memory;

public class TTLCacheExample
{
    private readonly IMemoryCache _cache;

    public TTLCacheExample(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GetCachedData(string key)
    {
        if (!_cache.TryGetValue(key, out string cachedData))
        {
            cachedData = FetchDataFromSource();
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            _cache.Set(key, cachedData, cacheOptions);
        }
        return cachedData;
    }

    private string FetchDataFromSource() => "Data from source";
}
```

**Use Case**: Stale data like tokens or weather updates.  
**Pros**: Automatic cleanup, prevents stale data.  
**Cons**: May evict useful data prematurely.

### Eviction Policies
Removes items when cache limits are reached, using policies like Least Recently Used (LRU).

```csharp
using StackExchange.Redis;

public class EvictionCacheExample
{
    private readonly IDatabase _redisDb;

    public EvictionCacheExample(ConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task ConfigureLRUEvictionAsync()
    {
        await _redisDb.ExecuteAsync("CONFIG", "SET", "maxmemory-policy", "allkeys-lru");
    }

    public async Task<string> GetCachedDataAsync(string key)
    {
        var cachedData = await _redisDb.StringGetAsync(key);
        if (cachedData.IsNull)
        {
            var data = FetchDataFromSource();
            await _redisDb.StringSetAsync(key, data, TimeSpan.FromMinutes(5));
            return data;
        }
        return cachedData;
    }

    private string FetchDataFromSource() => "Data from source";
}
```

**Use Case**: Memory-constrained systems.  
**Pros**: Optimizes memory usage.  
**Cons**: May evict frequently used data if not tuned properly.

## 🔍 Best Practices
- **Cache Invalidation**: Use tags or events to refresh cache on data updates.  
- **Thread Safety**: Ensure thread-safe cache access; `IMemoryCache` is thread-safe by default.  
- **Cache Stampede**: Use locks to prevent multiple threads from repopulating the same key.  
- **Monitoring**: Track hit/miss ratios to optimize performance.  
- **Scalability**: Prefer distributed caches for multi-server setups.  
- **Consistency**: Use write-through for strong consistency or write-behind for performance.
