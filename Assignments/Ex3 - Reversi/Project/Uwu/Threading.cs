using System;
using System.Threading;

namespace Uwu.Threads;

public static class Run
{
    /****************************************
    | Application Thread & Callback Routing |
    *****************************************/

    /**************************** [Synchronous Execution] *****************************/

    // Simple run-only synchronous execution of multiple actions.
    public static void RunAll(params Action?[] acts) { foreach (var a in acts) a?.Invoke(); }

    // Write-only: pass in parameters to all callbacks.
    public static void Write<S>(S s, params Action<S>?[] cbs) { foreach (var c in cbs) c?.Invoke(s); }
    public static void Write<R, S>(R r, S s, params Action<R, S>?[] cb) { foreach (var c in cb) c?.Invoke(r, s); }
    public static void Write<Q, R, S>(Q q, R r, S s, params Action<Q, R, S>?[] cbs) { foreach (var c in cbs) c?.Invoke(q, r, s); }

    // Read-only: Run all and collect return values, passing them to a reader / observer.
    public static void Read<T>(Action<T[]> reader, params Func<T>?[] writers)
    {
        Func<T>? fn;
        T[] responses = new T[writers.Length];
        for (int idx = 0; idx < writers.Length; idx++)
            responses[idx] = (fn = writers[idx] ?? default)!.Invoke();

        reader.Invoke(responses);
    }

    // Polling: Pass values in, then record return values for the observer. (1, 2, & 3 arg versions.)
    public static void Poll<S, T>(S s, Action<T[]> observer, params Func<S, T>?[] subject)
    {
        Func<S, T>? fn;
        T[] responses = new T[subject.Length];
        for (int idx = 0; idx < subject.Length; idx++)
            responses[idx] = (fn = subject[idx] ?? default)!.Invoke(s);

        observer.Invoke(responses);
    }

    public static void Poll<R, S, T>(R r, S s, Action<T[]> observer, params Func<R, S, T>?[] subject)
    {
        Func<R, S, T>? fn;
        T[] responses = new T[subject.Length];
        for (int idx = 0; idx < subject.Length; idx++)
            responses[idx] = (fn = subject[idx] ?? default)!.Invoke(r, s);

        observer.Invoke(responses);
    }

    public static void Poll<Q, R, S, T>(Q q, R r, S s, Action<T[]> observer, params Func<Q, R, S, T>?[] subject)
    {
        Func<Q, R, S, T>? fn; // Dumbest shit ever. This compiler...
        T[] responses = new T[subject.Length];
        for (int idx = 0; idx < subject.Length; idx++)
            responses[idx] = (fn = subject[idx] ?? default)!.Invoke(q, r, s);

        observer.Invoke(responses);
    }

    // Production variants (get live value, then write).
    public static void Produce<S>(Func<S> s, params Action<S>?[] cbs) => Write(s.Invoke(), cbs);
    public static void Produce<R, S>(Func<R> r, Func<S> s, params Action<R, S>?[] cbs) => Write(r.Invoke(), s.Invoke(), cbs);
    public static void Produce<Q, R, S>(Func<Q> q, Func<R> r, Func<S> s, params Action<Q, R, S>?[] cbs) =>
        Write(q.Invoke(), r.Invoke(), s.Invoke(), cbs);

    // Production-response variants (produce live value, then poll (write & read).
    public static void BuildRespond<S, T>(Func<S> s, Action<T[]> obs,
        params Func<S, T>?[] cbs) => Poll(s.Invoke(), obs, cbs);

    public static void BuildRespond<R, S, T>(Func<R> r, Func<S> s, Action<T[]> obs,
        params Func<R, S, T>?[] cbs) => Poll(r.Invoke(), s.Invoke(), obs, cbs);

    public static void BuildRespond<Q, R, S, T>(Func<Q> q, Func<R> r, Func<S> s, Action<T[]> obs,
        params Func<Q, R, S, T>?[] cbs) => Poll(q.Invoke(), r.Invoke(), s.Invoke(), obs, cbs);


    /**************************** [Asynchronous Execution] *****************************/

    // Simple variants: run-only, readonly, write-only.
    public static Thread RunSet(params Action?[] acts) => Asynchrify("Run-Set", true, acts);

    public static Thread ReadSet<T>(Action<T[]> reader, params Func<T>?[] writers) =>
        Asynchrify("Read-Set", true, () => { Read(reader, writers); });

    public static Thread WriteSet<S>(S s, params Action<S>?[] cbs) =>
        Asynchrify("WriteSet", true, () => Write(s, cbs));

    // Write-many variants.
    public static Thread WriteSet<R, S>(R r, S s, params Action<R, S>?[] cbs) =>
        Asynchrify("WriteSet", true, () => Write(r, s, cbs));

    public static Thread WriteSet<Q, R, S>(Q q, R r, S s, params Action<Q, R, S>?[] cbs) =>
        Asynchrify("WriteSet", true, () => Write(q, r, s, cbs));

    // Read-write variants
    public static Thread PollSet<S, T>(S s, Action<T[]> reader, params Func<S, T>?[] writers) =>
        Asynchrify("Poll-Set", true, () => { Poll(s, reader, writers); });

    public static Thread PollSet<R, S, T>(R r, S s, Action<T[]> reader, params Func<R, S, T>?[] writers) =>
        Asynchrify("Poll-Set", true, () => { Poll(r, s, reader, writers); });

    public static Thread PollSet<Q, R, S, T>(Q q, R r, S s, Action<T[]> reader, params Func<Q, R, S, T>?[] writers) =>
        Asynchrify("Poll-Set", true, () => { Poll(q, r, s, reader, writers); });

    // Production variants (get live value, then write).
    public static Thread ProduceSet<S>(Func<S> s, params Action<S>?[] cbs) => WriteSet(s.Invoke(), cbs);
    public static Thread ProduceSet<R, S>(Func<R> r, Func<S> s, params Action<R, S>?[] cbs) => WriteSet(r.Invoke(), s.Invoke(), cbs);
    public static Thread ProduceSet<Q, R, S>(Func<Q> q, Func<R> r, Func<S> s, params Action<Q, R, S>?[] cbs) =>
        WriteSet(q.Invoke(), r.Invoke(), s.Invoke(), cbs);

    // Production-response variants (produce live value, then poll (write & read).
    public static Thread BuildRespondSet<S, T>(Func<S> s, Action<T[]> obs,
        params Func<S, T>?[] cbs) => PollSet(s.Invoke(), obs, cbs);

    public static Thread BuildRespondSet<R, S, T>(Func<R> r, Func<S> s, Action<T[]> obs,
        params Func<R, S, T>?[] cbs) => PollSet(r.Invoke(), s.Invoke(), obs, cbs);

    public static Thread BuildRespondSet<Q, R, S, T>(Func<Q> q, Func<R> r, Func<S> s, Action<T[]> obs,
        params Func<Q, R, S, T>?[] cbs) => PollSet(q.Invoke(), r.Invoke(), s.Invoke(), obs, cbs);

    // Pass value around; look for exceptional response. Return original value if no exceptions.
    static T PassAround<T>(T item, T excp = default, params Func<T, T>?[] circle) where T : struct, IEquatable<T>
    {
        T response = item;
        foreach (var entity in circle)
            response = (response = entity?.Invoke(item) ?? default).Equals(excp) ? excp : response;

        return response;
    }

    // Send that task to the async farm! (Thread it separately)
    static Thread Asynchrify(string name, bool isBg = true, params Action?[] acts) =>
        new(() => { foreach (var a in acts) a?.Invoke(); }) { Name = name, IsBackground = isBg };
}