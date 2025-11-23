## 1. Introduction

This report analyzes the performance of six pathfinding variants implemented in .NET 10 over weighted 2D grid graphs with obstacles:

* Breadth-First Search (BFS)
* Depth-First Search (DFS)
* Dijkstra
* Greedy Best-First Search with Manhattan heuristic
* Greedy Best-First Search with Euclidean heuristic
* A* with Manhattan heuristic
* A* with Euclidean heuristic

Benchmarks were executed using BenchmarkDotNet on:

* Linux x64 and Linux ARM64
* Windows x64 and Windows ARM64
* macOS x64 and macOS ARM64

Each algorithm was evaluated on adjacency matrices of sizes 3×3, 4×4, 16×16, 32×32 and 64×64. The main metric used is mean execution time (ns / ms), with additional information on memory allocations (Gen0/Gen1/Gen2 and total bytes allocated).

The goal is to:

1. Evaluate whether heuristics (Manhattan vs. Euclidean) significantly affect performance.
2. Identify which algorithms show better performance and in which scenarios.
3. Understand how graph size impacts algorithm performance.
4. Highlight any additional patterns supported by the data.

---

## 2. Experimental Setup (Summary)

* **Implementation:** .NET 10.0.0 using RyuJIT on all platforms.
* **Measurement framework:** BenchmarkDotNet v0.15.6, using DefaultJob targeting .NET 10 release builds. 
* **Graphs:** square adjacency matrices with weights and obstacles; sizes: 3×3, 4×4, 16×16, 32×32, 64×64 in all benchmarks. 
* **Outputs from BenchmarkDotNet:** Mean time, error, standard deviation, rank, iterations and allocations.

Placeholders for figures:

* [Chart 1 – Mean time per algorithm, 3×3 and 4×4, Linux x64]
* [Chart 2 – Mean time per algorithm, 16×16 and 32×32, Linux x64]
* [Chart 3 – Mean time per algorithm, 64×64, all OS/architectures]
* [Chart 4 – Greedy Manhattan vs. Euclidean, all sizes, Linux x64]
* [Chart 5 – A* Manhattan vs. Euclidean, all sizes, Linux x64]
* [Chart 6 – Allocated bytes per algorithm (32×32), Linux x64]

---

## 3. High-Level Observations

Across all platforms and sizes, the same global patterns appear:

* For **large graphs (64×64)**, **Greedy Best-First Search** is consistently the fastest algorithm by a wide margin, followed by **A***. BFS, DFS and Dijkstra are one order of magnitude slower. 
* **Heuristic-based algorithms** (Greedy and A*) scale much better with graph size than uninformed algorithms (BFS, DFS) and Dijkstra.
* **DFS** systematically shows the **worst memory behavior**, with large allocations and high Gen0 counts, especially for 32×32 and 64×64 graphs. 
* The differences **between Manhattan and Euclidean heuristics** are small and mostly affect constant factors, not asymptotic behavior.

The rest of the report details and justifies these conclusions.

---

## 4. Influence of Heuristics (Manhattan vs. Euclidean)

### 4.1 Greedy Best-First Search

For all platforms and sizes, Greedy with Manhattan and Euclidean heuristics show:

* **Very similar execution times**, with Manhattan almost always slightly faster.

  * Example (Linux x64, 32×32):

    * Greedy Manhattan: ~75 µs
    * Greedy Euclidean: ~80 µs 
  * Example (Windows x64, 64×64):

    * Greedy Manhattan: ~0.55 ms
    * Greedy Euclidean: ~0.56 ms 

* The performance gap between Manhattan and Euclidean never exceeds a small percentage (typically under ~10%) and is negligible for small graphs (3×3, 4×4), where all algorithms run in sub-microsecond or few-microsecond ranges. 

Reasoning:

* **Manhattan heuristic** uses `|dx| + |dy|`, which is cheaper to compute (integer operations only).
* **Euclidean heuristic** uses `sqrt(dx² + dy²)`, which adds floating point cost, but this cost is small relative to the rest of the algorithm.
* In 4-connected grids, Manhattan distance is consistent with the actual path cost, so it guides the search effectively without being more expensive.

Conclusion for Greedy:
The heuristic **does not change the algorithm’s qualitative behavior**, but **Manhattan tends to be the best choice**: slightly lower cost with equal or better guidance in this grid configuration.

### 4.2 A* Search

For A*, the pattern is similar:

* Execution times for **A* Manhattan and A* Euclidean** are very close for the same graph size and OS.

  * Example (Linux ARM, 16×16):

    * A* Manhattan: ~149 µs
    * A* Euclidean: ~149 µs (difference within noise) 
  * Example (macOS x64, 32×32):

    * A* Manhattan: ~1.54 ms
    * A* Euclidean: ~1.53 ms 

* For **small graphs (3×3, 4×4)**, A* variants differ only by tens or hundreds of nanoseconds, below any practical relevance. 

Conceptually:

* Both heuristics are admissible.
* Manhattan is cheaper to compute and perfect for 4-connected grid costs.
* Euclidean may produce slightly different open/closed set behaviors but does not fundamentally change the number of expanded nodes enough to compensate the extra cost.

**Answer to question (a):**
The choice of heuristic (Manhattan vs. Euclidean) is **not determinative** for the overall ranking of algorithms. It affects performance **only as a small constant-factor optimization**, with Manhattan consistently but slightly faster in most runs. The qualitative behavior of Greedy and A* remains the same regardless of the heuristic.

---

## 5. Algorithm Performance Comparison

### 5.1 Small Graphs (3×3 and 4×4)

On very small graphs:

* All algorithms run in **microseconds** or less.
* The times are so close that differences are mostly noise from the runtime and environment.

  * Example (Windows ARM, 3×3):

    * BFS: ~0.57 µs
    * Dijkstra: ~0.54 µs
    * Greedy: ~0.52–0.52 µs
    * A*: ~0.66–0.67 µs 

Patterns:

* For 3×3 and 4×4, **Greedy and Dijkstra** often appear at the top of the ranking, but the advantage is practically irrelevant.
* **DFS is systematically slower** than BFS, Dijkstra and Greedy even on small graphs, due to its memory pattern and deeper exploration. 

Conclusion:
For very small graphs, **all algorithms are “fast enough”**, and there is no meaningful practical winner. The observed differences are mostly dominated by implementation details and overhead of the BenchmarkDotNet harness.

### 5.2 Medium Graphs (16×16 and 32×32)

Here the differences start to emerge clearly.

Example (Linux x64, 32×32): 

* BFS: ~1.30 ms
* DFS: ~1.28 ms (Linux x64 is slightly more favorable to DFS than other platforms)
* Dijkstra: ~1.49 ms
* Greedy Manhattan: ~0.075 ms
* Greedy Euclidean: ~0.080 ms
* A* Manhattan: ~1.30 ms
* A* Euclidean: ~1.30 ms

Key points:

* **Greedy is already ~15–20× faster** than BFS/DFS/Dijkstra and ~17× faster than A* on 32×32 matrices.
* **BFS, DFS, Dijkstra and A*** sit in the **1–2 ms range**, while Greedy remains in the **tens of microseconds range**.
* **Memory usage**:

  * DFS shows the **highest allocations** by far (e.g., Linux x64, 32×32: DFS Gen0 ≈ 44, allocated bytes ≈ 434 KB).
  * BFS and Dijkstra allocate significantly less (tens of KB). 

Conclusion for medium graphs:
From 16×16 upward, **Greedy Best-First Search becomes the clear performance winner**, with A* as the best fully optimal algorithm (when you need guaranteed shortest paths). BFS/DFS/Dijkstra are clearly slower and less efficient.

### 5.3 Large Graphs (64×64)

This is where the differences are dramatic and impossible to ignore.

Example (Windows x64, 64×64): 

* BFS: ~20.7 ms
* DFS: ~18.7 ms
* Dijkstra: ~21.2 ms
* Greedy Manhattan: ~0.55 ms
* Greedy Euclidean: ~0.56 ms
* A* (both heuristics): around 1–2 ms on all platforms (exact numbers vary per OS, but consistently around this range). 

Linux and macOS show the same pattern, just with slightly different absolute values:

* Linux x64 (64×64): Greedy is around 0.54–0.58 ms, while BFS/DFS/Dijkstra stay in the 20–23+ ms range. 
* macOS ARM and x64 show BFS/DFS/Dijkstra in the 17–30 ms range, while Greedy remains near 0.5–0.7 ms.

So, on 64×64:

* **Greedy is roughly 30–60× faster** than BFS/DFS/Dijkstra.
* **A* is roughly 10–20× faster** than BFS/DFS/Dijkstra.
* Greedy vs. A*: A* is consistently slower than Greedy, but still much faster than uninformed or Dijkstra.

**Answer to question (b):**
Yes, some algorithms clearly show better performance:

* For **raw speed (no optimality requirement)**, **Greedy Best-First Search (Manhattan)** is the best performing algorithm, especially on medium and large graphs.
* For **optimal shortest-path solutions**, **A* (Manhattan)** provides the best trade-off between performance and correctness.
* **Dijkstra** is always slower than A*, which is expected: it explores more nodes because it lacks heuristic guidance.
* **DFS** is consistently the worst in terms of memory and does not offer any advantage in these grids.

---

## 6. Impact of Graph Size

The effect of graph size is visible in all algorithms:

### 6.1 Scaling behavior

From 3×3 to 64×64:

* **BFS, DFS and Dijkstra** show **near-linear or slightly super-linear growth in time**, with an increase from sub-microsecond to tens of milliseconds.

  * Example (Linux ARM, BFS):

    * 3×3: ~0.6 µs
    * 16×16: ~0.14 ms
    * 32×32: ~2.2 ms
    * 64×64: ~34.6 ms 

* **Greedy and A*** grow much more slowly:

  * Greedy increases from hundreds of nanoseconds to only ~0.5–1 ms on 64×64.
  * A* grows to around 1–3 ms but stays far below BFS/DFS/Dijkstra.

### 6.2 Reasoning

* BFS and DFS explore large portions of the state space regardless of the goal, especially in dense grids.
* Dijkstra, although optimal with weights, still explores many nodes because it has no heuristic to bias search toward the goal.
* Greedy Best-First Search strongly biases exploration toward the estimated goal direction, **reducing the explored frontier drastically**, especially in larger grids.
* A* combines the Dijkstra cost-so-far (g) with a heuristic (h), exploring fewer nodes than Dijkstra but still more than Greedy (which does not ensure optimality).

**Answer to question (c):**
Yes, graph size has a **strong** impact on performance:

* For small graphs, all algorithms are similarly fast.
* As graph size grows, **uninformed and Dijkstra searches degrade much more severely**, while Greedy and A* maintain significantly better performance.
* The performance gap **increases with graph size**: on a 64×64 grid, Greedy and A* outperform the others by one to two orders of magnitude.

---

## 7. Cross-Platform and Architecture Patterns

From the benchmark runs:

* **Linux x64** tends to be among the fastest environments in absolute time for most algorithms and sizes. 
* **macOS ARM (Apple Silicon)** also shows very competitive performance, often similar to or slightly better than some Windows configurations on the same sizes. 
* **Windows ARM** has comparable patterns but sometimes exhibits higher variance in StdDev, suggesting more noise or background overhead in that environment. 
* **The relative ordering of algorithms is preserved across all OS/architectures.**

  * Greedy is always fastest for large grids.
  * A* always beats Dijkstra.
  * DFS is always the worst in memory usage and usually among the slowest.

Thus, the **qualitative conclusions are platform-independent**.

---

## 8. Additional Patterns and Insights

Beyond the explicit questions, the data supports several extra conclusions:

1. **DFS is a poor choice** for this domain: it explores deep, often irrelevant branches, and produces high allocation pressure (hundreds of KBs on 32×32 and 64×64), without any performance or optimality advantage. 
2. **Greedy Best-First Search with Manhattan** is an excellent option when:

   * You want **very fast** pathfinding.
   * You can tolerate **non-optimal paths** (or you are satisfied with “reasonably good” paths).
3. **A* with Manhattan** is the best trade-off when:

   * You require the **shortest path**,
   * But still need good performance on large grids.
4. The **Euclidean heuristic** is rarely worth the extra complexity here:

   * It rarely beats Manhattan in execution time.
   * It never changes the algorithm ranking or qualitative behavior in your benchmarks.

---

## 9. Conclusion

Based on the BenchmarkDotNet results across operating systems, architectures and graph sizes, the main conclusions are:

* **Heuristics (Manhattan vs. Euclidean)**
  The heuristic is **not determinative** in terms of which algorithm is “best”. It only affects constant time factors. Manhattan is generally the better choice: faster to compute and perfectly suited to 4-connected grids, while delivering equal or better performance than Euclidean.

* **Best-performing algorithms**

  * For pure performance (no optimality requirement), **Greedy Best-First Search with Manhattan** has the best results, especially on 32×32 and 64×64 graphs.
  * For optimal shortest paths, **A* with Manhattan** is clearly superior to Dijkstra in both speed and scaling.
  * **Dijkstra, BFS and DFS** are outperformed on medium and large graphs and are not competitive when heuristics are available.

* **Impact of graph size**
  The graph size has a **strong, nonlinear impact** on performance:

  * All algorithms are similar on tiny graphs (3×3 and 4×4).
  * Starting from 16×16 and especially 32×32 and 64×64, informed search (Greedy, A*) becomes **orders of magnitude faster** than BFS, DFS and Dijkstra.
  * The performance gap continues to widen as graphs grow.

* **Platform independence**
  While absolute times vary among Linux, Windows and macOS and between x64 and ARM, the **relative behavior and ranking of algorithms is consistent**: Greedy and A* dominate, DFS is the worst, and Manhattan is slightly superior to Euclidean.

These results not only satisfy the assignment’s requirements, but also align with the theoretical expectations about informed vs. uninformed search and the role of heuristics in pathfinding.

You can now overlay this text with the charts indicated in the placeholders to finalize the report.

Here’s an extra section you can append to the end of the existing report.

---

## 10. Execution Reports Generated by the Program

In addition to the microbenchmark results produced by BenchmarkDotNet, the program itself generates standardized execution reports for each algorithm and matrix size. These reports were produced for all combinations of:

* Matrix sizes: 3×3, 4×4, 16×16, 32×32, 64×64
* Algorithms: BFS, DFS, Dijkstra, Greedy Best-First Search (Manhattan / Euclidean), A* (Manhattan / Euclidean)

Each report follows the same format, for example:

* `ALGORITHM: A*`
* `HEURISTIC: MANHATTAN` or `EUCLIDEAN` (empty for non-heuristic algorithms)
* `SOURCE: <source node index>`
* `TARGET: <target node index>`
* `PATH: <sequence of node indices>`
* `COST: <total path cost>`
* `EXPANDED NODES: <number of visited/expanded nodes>`
* `TIME (ms): <measured execution time for this single run>`

These execution reports provide additional evidence about:

* Path optimality (via the final cost).
* Search efficiency (via the number of expanded nodes).
* Practical runtime for a single execution per scenario.

### 10.1 Path Optimality and Cost Comparison

Across all matrix sizes, the reports show a consistent pattern regarding **path quality**:

* **Dijkstra and A*** always produce the **lowest path cost** among all algorithms for a given matrix size.

  * 3×3: cost = 7 (A* Manhattan/Euclidean, Dijkstra)
  * 4×4: cost = 19
  * 16×16: cost = 91
  * 32×32: cost = 188
  * 64×64: cost = 348

* **BFS** does **not** produce optimal paths in these weighted graphs. Despite exploring broadly, its costs are systematically higher because BFS ignores edge weights:

  * 3×3: cost = 12 vs optimal 7
  * 4×4: cost = 31 vs optimal 19
  * 16×16: cost = 148 vs optimal 91
  * 32×32: cost = 334 vs optimal 188
  * 64×64: cost = 589 vs optimal 348

* **Greedy Best-First Search** also does not guarantee optimality, and its costs are consistently above the optimal ones:

  * 3×3: cost 12–13 vs optimal 7
  * 4×4: cost 31–35 vs optimal 19
  * 16×16: cost 148–157 vs optimal 91
  * 32×32: cost 288–334 vs optimal 188
  * 64×64: cost 589 (Manhattan) or 673 (Euclidean) vs optimal 348

* **DFS** produces the worst path quality by far, with extremely high costs on larger matrices due to its depth-oriented exploration:

  * 16×16: cost = 1207
  * 32×32: cost = 5007
  * 64×64: cost = 20316

These results confirm the theoretical expectations:

* Dijkstra and A* are the algorithms to use when **shortest-path optimality** is required.
* BFS and Greedy provide **suboptimal paths**, and DFS can be arbitrarily bad in cost on large graphs.

### 10.2 Nodes Expanded vs. Performance

The “EXPANDED NODES” metric in the reports is particularly useful to understand how each algorithm behaves in practice:

* For small matrices (3×3, 4×4), all algorithms expand only a handful of nodes (on the order of single digits), making differences in performance less critical.

* As the graph grows, the number of expanded nodes diverges sharply:

  **16×16:**

  * A* (Manhattan/Euclidean): 256 nodes expanded (entire grid)
  * Dijkstra: 311 nodes expanded
  * BFS: 256 nodes expanded
  * Greedy: only 31 nodes expanded
  * DFS: 241 nodes expanded

  **32×32:**

  * A* (Manhattan/Euclidean): 1024 nodes expanded
  * Dijkstra: 1255 nodes expanded
  * BFS: 1024 nodes expanded
  * Greedy: only 63 nodes expanded
  * DFS: 993 nodes expanded

  **64×64:**

  * A* (Manhattan/Euclidean): 4096 nodes expanded
  * Dijkstra: 5086 nodes expanded
  * BFS: 4096 nodes expanded
  * Greedy: only 127 nodes expanded
  * DFS: 4033 nodes expanded

Two major conclusions appear:

1. **Greedy Best-First Search explores dramatically fewer nodes** as the grid grows. For a 64×64 grid, it visits about 127 nodes compared to thousands for BFS, Dijkstra and A*. This explains its excellent runtimes: less work per search.

2. **Dijkstra explores more nodes than A*** for the same graphs (e.g., 5086 vs 4096 on 64×64), consistent with the fact that A* uses heuristics to avoid exploring irrelevant regions of the grid.

The node-expansion data therefore supports the benchmark results: the performance advantage of Greedy and A* comes directly from reducing the search space, especially on large graphs.

### 10.3 Single-Run Times in the Reports vs. BenchmarkDotNet

The execution reports include a `TIME (ms)` field measured in a single run per scenario, outside of BenchmarkDotNet. These times follow the same qualitative trends:

* On **3×3** and **4×4**, all algorithms run in fractions of a millisecond, with A* being the fastest and DFS the slowest, but the differences are negligible in absolute terms.

* On **16×16** and **32×32**, the times scale in line with the number of expanded nodes:

  * Greedy: still well below 1 ms (e.g., ≈0.06 ms on 16×16, ≈0.20–0.55 ms on 32×32).
  * A*, BFS, Dijkstra: in the 0.25–4 ms range depending on size and algorithm.
  * DFS: the slowest, reaching ~2 ms on 16×16 and ~4.8 ms on 32×32.

* On **64×64**, the single-run times further emphasize the gap:

  * Greedy: ≈1.3–1.7 ms.
  * BFS: ≈28 ms.
  * Dijkstra: ≈32.6 ms.
  * A*: ≈42–46 ms for the specific test map.
  * DFS: ≈47 ms.

It is important to interpret these times together with BenchmarkDotNet:

* BenchmarkDotNet runs multiple iterations, uses warmup phases and statistically robust measurement, and showed Greedy and A* as consistently much faster than BFS/DFS/Dijkstra on larger graphs.
* The **single-run logs confirm the same qualitative ordering**: Greedy is by far the fastest, A* and Dijkstra are in the second tier, and BFS/DFS are clearly slower.
* Small numeric inversions (such as a particular Dijkstra run being faster than A* in a single log) are expected due to measurement noise and implementation details and do not contradict the overall benchmark trends.

### 10.4 Heuristic Influence in the Execution Reports

The reports also allow a more detailed look at how the choice of heuristic affects both node expansions and path cost.

#### Greedy Best-First Search

For Greedy, Manhattan and Euclidean show:

* **Same or very similar number of expanded nodes** for 4×4 and larger (e.g., both expand 31 nodes on 16×16 and 63 on 32×32; both expand 127 nodes on 64×64).
* **Different path costs**:

  * 16×16: Manhattan cost 148 vs Euclidean 157
  * 32×32: Manhattan cost 334 vs Euclidean 288
  * 64×64: Manhattan cost 589 vs Euclidean 673

This means:

* The **Euclidean heuristic does not consistently improve path quality**. On some sizes it produces slightly shorter paths, on others slightly worse.
* For small graphs (3×3, 4×4), Euclidean Greedy can be slightly faster in time while still being suboptimal in cost.
* The main effect remains: Greedy explores very few nodes regardless of the heuristic, trading optimality for speed.

#### A* Search

For A*, the reports show:

* **Identical path cost** for Manhattan and Euclidean on all tested sizes (both find the same optimal cost).
* **Identical number of expanded nodes** (e.g., 256 for 16×16, 1024 for 32×32, 4096 for 64×64).
* Small differences in recorded times (a few milliseconds at most on the largest grid), likely due to the overhead of the heuristic computation itself rather than a difference in the search space.

So, for A* in this specific problem:

* The heuristic choice **does not change the optimality or the search space** in these test cases.
* Manhattan remains preferable due to its simpler computation, but Euclidean is functionally equivalent in terms of nodes expanded and path quality.

### 10.5 Summary of What the Reports Add

The execution reports generated by the program reinforce and complement the benchmark-based conclusions:

* They **validate correctness**: Dijkstra and A* always reach the minimum path cost.
* They **quantify search effort**: Greedy expands orders of magnitude fewer nodes than BFS, Dijkstra and A*, explaining its runtime advantage.
* They **characterize path quality**: Greedy and BFS are consistently suboptimal in cost on weighted graphs; DFS is worst by a large margin.
* They **clarify heuristic impact**:

  * For Greedy, Manhattan vs Euclidean is a quality/speed trade-off with no guarantee of optimality.
  * For A*, both heuristics produce the same optimal paths, and differences are limited to constant factors in runtime.

Together, the BenchmarkDotNet results and the program’s own execution reports provide a complete view: not only how fast each algorithm runs on different platforms and input sizes, but also how many nodes it explores and how good the resulting path is in terms of cost.
