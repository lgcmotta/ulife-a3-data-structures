# Relatório de Desempenho de Algoritmos de Busca de Caminho

Este relatório apresenta uma análise integrada de cinco algoritmos de busca de caminho implementados em **.NET 10**.
Dois desses algoritmos (Greedy e A*) utilizam heurísticas de Manhattan e Euclidiana, resultando em um total de **sete cenários principais**.  
Cada algoritmo foi testado com matrizes de adjacência de tamanhos: $3\times3$, $4\times4$, $16\times16$, $32\times32$ e $64\times64$.

- Breadth-First Search (BFS) — Busca em Largura
- Depth-First Search (DFS) — Busca em Profundidade
- Dijkstra
- Greedy Best-First Search (GBFS) — Busca Gulosa de Melhor Primeiro
  - Heurística de Manhattan
  - Heurística Euclidiana
- A*
  - Heurística de Manhattan
  - Heurística Euclidiana

Duas fontes de dados complementares foram utilizadas durante a análise:

1. Micro-benchmarks usando [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet), executados em **Linux**, **macOS** e **Windows**, nas arquiteturas **x64** e **ARM**.
2. Relatórios de execução gerados pela aplicação principal, contendo:
   - **COST**: custo total do caminho computado.
   - **EXPANDED NODES**: número de nós visitados.
   - **PATH**: a sequência de nós na rota resultante.
   - **TIME**: duração da execução para uma única execução.

Juntos, esses conjuntos de dados fornecem uma compreensão completa de várias dimensões do comportamento algorítmico:

- Desempenho  
- Otimalidade  
- Influência heurística  
- Escalabilidade com o tamanho do grafo  
- Exploração do espaço de busca  
- Consistência entre plataformas  

---

## Metodologia

### Representação do Grafo

A entrada é uma **matriz de adjacência**, onde cada linha e coluna corresponde a um nó no grafo.  
O valor na posição $(i, j)$ corresponde ao custo da aresta conectando o nó $i$ ao nó $j$:

- Se o valor for $\gt 0$, existe uma aresta válida com esse peso.
- Se o valor for $\leq 0$, não existe conexão direta entre os nós.

Esta representação **não** descreve uma grade com obstáculos; ao invés disso, define um grafo ponderado cuja conectividade é inteiramente determinada pela matriz de adjacência.

### Testes dos Algoritmos

Todos os algoritmos foram implementados usando a mesma estrutura de grafo e avaliados com:

- Adjacência de quatro direções, consistente com as arestas definidas na matriz.
- Os mesmos nós de início e destino em todos os testes.
- Tamanhos de matriz idênticos ($3\times3$ a $64\times64$).
- O mesmo modelo de custo e formato de entrada.

Os testes com BenchmarkDotNet foram executados usando [GitHub Actions](https://github.com/features/actions) em seis configurações de plataforma:

| SO      | Arquitetura | Versão               | CPU                                                                    |
|---------|:-----------:|----------------------|------------------------------------------------------------------------|
| Linux   | ARM         | Ubuntu 24.04.3 LTS   | Neoverse-N2, 4 núcleos físicos                                         | 
| Linux   | x64         | Ubuntu 24.04.3 LTS   | AMD EPYC 7763 2.45GHz, 1 CPU, 4 núcleos lógicos e 2 núcleos físicos    |
| macOS   | ARM         | macOS Sequoia 15.7.1 | Apple M1 (Virtual), 1 CPU, 3 núcleos lógicos e 3 núcleos físicos       |
| macOS   | x64         | macOS Sequoia 15.7.1 | Intel Core i7-8700B CPU 3.20GHz, 1 CPU, 4 núcleos lógicos e 4 núcleos físicos |
| Windows | ARM         | Windows 11           | Cobalt 100 3.40GHz, 1 CPU, 4 núcleos lógicos e 4 núcleos físicos       |
| Windows | x64         | Windows 11           | AMD EPYC 7763 2.44GHz, 1 CPU, 4 núcleos lógicos e 2 núcleos físicos    |

A estrutura do relatório de execução gerado por algoritmo é:

```txt
ALGORITMO: <nome do algoritmo>
HEURÍSTICA: <Manhattan / Euclidiana / vazio>
ORIGEM: <vértice de origem>
DESTINO: <vértice de destino>
CAMINHO: <sequência de índices de nós>
CUSTO: <soma dos pesos>
NÓS EXPANDIDOS: <contagem>
TEMPO (ms): <tempo de execução única>
```

---

## Visão Geral dos Resultados Integrados

Tanto os benchmarks quanto os relatórios de execução revelam uma hierarquia consistente de desempenho e qualidade:

### Greedy Best-First Search

Algoritmo mais rápido, visitando extremamente poucos nós, mas o caminho resultante é frequentemente não ótimo devido à dependência completa da heurística.

### A* 

- Sempre produz caminhos ótimos.
- Significativamente mais rápido que Dijkstra graças à orientação heurística, oferecendo o melhor equilíbrio entre velocidade e otimalidade.

### Dijkstra

Garante otimalidade, mas explora mais nós que A*, tornando-o mais lento na prática.

### BFS

Ignora pesos, produzindo resultados subótimos em grafos ponderados, e explora grandes porções do espaço de busca.

### DFS

Produz a pior qualidade de caminho, explora profundamente e ineficientemente, e tem o pior desempenho em matrizes maiores.

Essas conclusões permaneceram estáveis em todos os sistemas operacionais e arquiteturas de hardware testados.

---

## Análise Heurística (Manhattan vs Euclidiana)

A função de avaliação usada para determinar qual nó deve ser explorado a seguir é dada por:

$$
f(n) = g(n) + h(n)
$$

Onde:
- $n \in V$ é um vértice no grafo.
- $g(n)$ é o custo acumulado do nó inicial até $n$:

$$
g(n) = \sum_{(u,v) \in P_{start \rightarrow n}} w(u, v)
$$

- $P_{start \rightarrow n}$ é a sequência ordenada de arestas ao longo do caminho descoberto.
- $(u, v) \in E$ é uma aresta conectando os nós $u$ e $v$.
- $w(u, v)$ é o peso associado à aresta $(u, v)$, representando o custo de travessia.
- $h(n)$ é a estimativa heurística da distância restante entre o nó $n$ e o objetivo:

$$
h : V \rightarrow \mathbb{R}_{\ge 0}
$$

O valor $f(n)$ determina a prioridade do nó $n$ na lista aberta, influenciando diretamente a direção da busca.

### Greedy Best-First Search

O algoritmo **GBFS** usa apenas o componente heurístico:

$$
f(n) = h(n)
$$

Como ignora o custo acumulado $g(n)$, o GBFS se comporta de forma similar a uma busca de "seguir o farol": 
Ele simplesmente se move na direção que parece mais próxima do objetivo de acordo com a heurística. 

Isso leva a:

- Exploração mínima do espaço de busca.
- Desempenho de alta velocidade.
- Qualidade de caminho altamente variável.
- Forte dependência da heurística escolhida.

A inconsistência entre heurísticas fica clara na tabela de custo de caminho:

|     Tamanho  | Manhattan | Euclidiana |
|:------------:|:---------:|:----------:|
|  $3\times3$  |    12     |     13     |
|  $4\times4$  |    31     |     35     |
| $16\times16$ |    148    |    157     |
| $32\times32$ |    334    |    228     |
| $64\times64$ |    589    |    673     |

![gbfs_path_cost_comparison](images/gbfs_path_cost_comparison.png)

#### Nós Expandidos (GBFS)

Apesar das diferenças no custo de caminho entre Manhattan e Euclidiana, ambas as heurísticas expandiram **o mesmo número de nós** em todos os cenários. 
Isso acontece porque o GBFS seleciona nós apenas com base no valor heurístico $h(n)$, sem considerar o custo acumulado $g(n)$. 

Embora Manhattan e Euclidiana possam atribuir diferentes valores heurísticos a cada nó, sua ordenação relativa nestes grafos semelhantes a grades frequentemente leva o GBFS a seguir a mesma sequência de nós explorados.
Como resultado, o **espaço de busca explorado pelo GBFS permanece idêntico** para ambas as heurísticas, mesmo que os **caminhos finais possam diferir significativamente no custo total**. 

Isso reforça uma das características principais do GBFS: A heurística influencia fortemente a rota escolhida, mas não necessariamente aumenta ou diminui o número de nós visitados. 
Em vez disso, ela altera principalmente a *direção* da busca, não sua *amplitude*.


|     Tamanho  | Nós Expandidos |
|:------------:|:--------------:|
| $16\times16$ |       31       |
| $32\times32$ |       63       |
| $64\times64$ |      127       |

### A* Search

O algoritmo **A*** usa a função de avaliação completa:

$$
f(n) = g(n) + h(n)
$$

Como mantém tanto o custo acumulado quanto as estimativas heurísticas, o A* pode reduzir o número de nós expandidos enquanto ainda garante otimalidade em grafos ponderados. 

Neste projeto:

- Ambas as heurísticas de Manhattan e Euclidiana produziram custos de caminho idênticos em todos os tamanhos de matriz.
- Diferenças foram observadas apenas na sobrecarga computacional, com Manhattan sendo ligeiramente mais rápido.

Resultados ótimos:

|     Tamanho  | Manhattan | Euclidiana |
|:------------:|:---------:|:----------:|
|  $3\times3$  |     7     |     7      |
|  $4\times4$  |    19     |    19      |
| $16\times16$ |    91     |    91      |
| $32\times32$ |    188    |    188     |
| $64\times64$ |    348    |    348     |

![astar_path_cost_comparison](images/astar_path_cost_comparison.png)

#### Nós Expandidos (A*)

No caso do A*, ambas as heurísticas produziram custos de caminho idênticos e expandiram o mesmo número de nós em todos os tamanhos de grafo. 
Este comportamento emerge porque Manhattan e Euclidiana são ambas heurísticas **admissíveis** e **consistentes** em grafos semelhantes a grades, significando que nunca superestimam a distância real até o objetivo e preservam a condição de monotonicidade requerida para a busca A* ótima.

Como ambas as heurísticas geram valores de prioridade $f(n) = g(n) + h(n)$ que diferem apenas por um fator de escala constante, sua ordenação de nós no conjunto aberto permanece efetivamente a mesma. 

Como consequência, o A* explora o mesmo espaço de busca independentemente de usar a distância de Manhattan ou Euclidiana. As duas heurísticas influenciam o tempo de computação apenas através de sua complexidade aritmética, não através da estrutura da busca.

Na prática, Manhattan oferece uma ligeira vantagem de desempenho devido ao menor custo computacional, enquanto Euclidiana não produz nenhuma redução nos nós explorados. 

Por essa razão, confirma-se que, para o A*, **a escolha entre Manhattan e Euclidiana afeta o tempo de execução mas não a corretude, otimalidade ou amplitude da busca**.

|     Tamanho  | Nós Expandidos |
|:------------:|:--------------:|
| $16\times16$ |      256       |
| $32\times32$ |     1024       |
| $64\times64$ |     4096       |

---

## Análise Algoritmo por Algoritmo

Esta seção apresenta uma comparação detalhada de cada algoritmo, combinando relatórios de execução (`COST` e `EXPANDED NODES`) com resultados de microbenchmark do BenchmarkDotNet. 
O objetivo é analisar como cada algoritmo se comporta na prática, como interage com a estrutura do grafo, e como suas decisões de design influenciam desempenho e otimalidade.

### Breadth-First Search (BFS)

O BFS trata todas as arestas como tendo o mesmo custo, o que o torna ótimo apenas em grafos não ponderados. 
Como as matrizes de adjacência usadas neste projeto contêm pesos arbitrários de arestas, o BFS inevitavelmente produz caminhos subótimos. 
Sua tendência de explorar vizinhos uniformemente leva a espaços de busca maiores, especialmente em matrizes maiores.

|     Tamanho  | Custo de Caminho | Nós Expandidos |
|:------------:|:----------------:|:--------------:|
|  $3\times3$  |        12        |     Pequeno    |
|  $4\times4$  |        31        |     Médio      |
| $16\times16$ |       148        |      256       |
| $32\times32$ |       334        |      1024      |
| $64\times64$ |       589        |      4096      |

O BFS é direto mas ineficiente para grafos ponderados. Ele expande sistematicamente grandes porções do grafo, resultando em tempos de execução maiores e uso significativo de memória.

![bfs_search_space](images/bfs_search_space.png)

----

### Depth-First Search (DFS)

O DFS segue um único caminho o mais profundamente possível antes de retroceder. Embora essa estratégia possa ser simples de implementar, é altamente inadequada para busca de caminho:
- Produz os piores custos de caminho entre todos os algoritmos.
- Expande regiões profundas do grafo com pouca consideração pela direção.
- Seu comportamento é extremamente sensível ao layout do grafo.

|     Tamanho  | Custo de Caminho | Nós Expandidos |
|:------------:|:----------------:|:--------------:|
|  $3\times3$  |    Muito alto    |  Pequeno/Médio |
|  $4\times4$  |    Muito alto    |     Médio      |
| $16\times16$ |       1207       |      241       |
| $32\times32$ |       5007       |      993       |
| $64\times64$ |      20316       |      4033      |

![dfs_path_cost_comparison](images/dfs_path_cost_comparison.png)

----

### Dijkstra

Dijkstra é um algoritmo clássico de caminho mais curto. Garante otimalidade mas não usa heurísticas, o que faz com que explore mais nós do que o necessário.

|     Tamanho  | Custo de Caminho | Nós Expandidos |
|:------------:|:----------------:|:--------------:|
|  $3\times3$  |        7         |     Pequeno    |
|  $4\times4$  |        19        |     Médio      |
| $16\times16$ |        91        |      311       |
| $32\times32$ |       188        |      1255      |
| $64\times64$ |       348        |      5086      |

Mesmo que Dijkstra seja ótimo, ele é consistentemente mais lento que A*, especialmente conforme o tamanho da matriz aumenta. 
Sua falta de orientação heurística causa expansão extensiva do espaço de busca.

![dijkstra_vs_astar_expanded_nodes](images/dijkstra_vs_astar_expanded_nodes.png)

----

### Greedy Best-First Search (GBFS)

O GBFS prioriza o nó com o menor valor heurístico. Isso produz velocidade extrema, mas não necessariamente caminhos ótimos. 
Como mostrado anteriormente, o número de nós expandidos permanece o mesmo para ambas as heurísticas de Manhattan e Euclidiana.

|     Tamanho  | Custo de Caminho (Manhattan) | Custo de Caminho (Euclidiana) | Nós Expandidos |
|:------------:|:----------------------------:|:-----------------------------:|:--------------:|
|  $3\times3$  |              12              |              13               |     Pequeno    |
|  $4\times4$  |              31              |              35               |  Pequeno/Médio |
| $16\times16$ |             148              |              157              |       31       |
| $32\times32$ |             334              |              228              |       63       |
| $64\times64$ |             589              |              673              |      127       |

O GBFS é excelente para aproximações rápidas mas não pode ser confiável para soluções ótimas. Seu desempenho aumenta dramaticamente com o tamanho do grafo, mas a precisão diminui.

![gbfs_cost_variability](images/gbfs_cost_variability.png)

----

### A* Search

O A* combina o custo de caminho acumulado $g(n)$ com a heurística $h(n)$ para equilibrar direcionalidade e exploração ótima. Neste projeto:

- Ambas as heurísticas produziram caminhos ótimos idênticos.
- Os nós expandidos permaneceram os mesmos para ambas as heurísticas.
- Manhattan foi ligeiramente mais rápido devido a um cálculo de distância mais simples.

|     Tamanho  | Custo de Caminho (Manhattan/Euclidiana) | Nós Expandidos |
|:------------:|:---------------------------------------:|:--------------:|
|  $3\times3$  |                    7                    |     Pequeno    |
|  $4\times4$  |                   19                    |     Médio      |
| $16\times16$ |                   91                    |      256       |
| $32\times32$ |                  188                    |      1024      |
| $64\times64$ |                  348                    |      4096      |

O A* oferece o melhor equilíbrio entre desempenho e precisão entre todos os algoritmos avaliados.

![astar_performance_scaling](images/astar_performance_scaling.png)

---

## Comportamento de Escala

O comportamento de escala reflete como o tempo de execução, o tamanho do espaço de busca e a complexidade algorítmica evoluem conforme a matriz cresce.

### Escalabilidade do Tempo de Execução

Os resultados do BenchmarkDotNet revelam a seguinte tendência geral:

- O GBFS é consistentemente o mais rápido, com tempo de execução crescendo quase logaritmicamente.
- O A* aumenta linearmente com o tamanho do grafo, refletindo seu uso equilibrado de custo e heurística.
- BFS e DFS experimentam crescimento substancial devido aos grandes espaços de busca que percorrem.
- Dijkstra cresce mais rápido que A* devido à sua falta de poda heurística.

![execution_time_vs_size_all_algorithms](images/execution_time_vs_size_all_algorithms.png)

---

### Escalabilidade do Espaço de Busca (Nós Expandidos)

O número de nós explorados cresce da seguinte forma:

|     Tamanho  | GBFS |   A* |  BFS | Dijkstra |  DFS |
|:------------:|-----:|-----:|-----:|---------:|-----:|
| $16\times16$ |   31 |  256 |  256 |      311 |  241 |
| $32\times32$ |   63 | 1024 | 1024 |     1255 |  993 |
| $64\times64$ |  127 | 4096 | 4096 |     5086 | 4033 |

O GBFS cresce mais lentamente, enquanto BFS e A* crescem identicamente devido à estrutura uniforme do grafo de matriz. Dijkstra e DFS expandem ainda mais nós.

![nodes_expanded_vs_size](images/nodes_expanded_vs_size.png)

---

### Divergência do Custo de Caminho

O custo de caminho se torna um indicador claro de precisão:

- A* e Dijkstra permanecem ótimos.
- BFS e GBFS desviam significativamente conforme o tamanho aumenta.
- DFS cresce explosivamente e de forma imprevisível.

![path_cost_vs_optimal_all_algorithms](images/path_cost_vs_optimal_all_algorithms.png)

---

## Análise Multiplataforma

Apesar das diferenças arquiteturais entre tipos de CPU e sistemas operacionais, todos os comportamentos relativos permaneceram estáveis:

- O GBFS foi o algoritmo mais rápido em todas as plataformas.
- O A* consistentemente venceu Dijkstra.
- BFS e DFS consistentemente tiveram o pior desempenho.
- Manhattan permaneceu ligeiramente mais rápido que Euclidiana devido a cálculos heurísticos mais baratos.
- Nenhum algoritmo mostrou desvios dependentes de plataforma em corretude, nós expandidos ou custo de caminho.

Os tempos de execução absolutos variaram ligeiramente:

- Linux x64 e macOS ARM tenderam a produzir os tempos mais baixos.
- Windows ARM exibiu a maior variância.
- macOS x64 permaneceu estável mas ligeiramente mais lento que Linux.

Essas diferenças, no entanto, não mudaram as conclusões algorítmicas.

![cross_platform_execution_time_comparison](images/cross_platform_execution_time_comparison.png)

---

## Conclusão

Este estudo fornece uma avaliação completa de cinco algoritmos principais de busca de caminho (sete cenários totais incluindo heurísticas) em múltiplos tamanhos de matriz de adjacência e plataformas de execução. 
A combinação de dados de micro-benchmark e logs de execução detalhados nos permite avaliar cada algoritmo tanto de perspectivas teóricas quanto práticas.

As conclusões mais importantes são:

- O A* oferece o melhor equilíbrio entre desempenho e otimalidade.
- O GBFS alcança desempenho inigualável mas sacrifica confiabilidade na qualidade do caminho.
- Dijkstra é sempre ótimo mas ineficiente sem orientação heurística.
- BFS é inadequado para grafos ponderados.
- DFS é a pior escolha para busca de caminho e escala mal.
- As heurísticas de Manhattan e Euclidiana se comportam diferentemente no GBFS mas identicamente no A*.
- O comportamento multiplataforma é estável e consistente.

No geral, para aplicações que requerem soluções ótimas, A* é o algoritmo recomendado.
Para cenários que priorizam velocidade sobre precisão, GBFS oferece excelente desempenho.

![summary_infographic](images/summary_infographic.png)
