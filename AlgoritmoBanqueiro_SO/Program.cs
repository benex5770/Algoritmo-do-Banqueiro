
class Banqueiro
{
  
    static readonly int TOTAL_CLIENTES = 5;
    static readonly int TOTAL_RECURSOS = 3;

    int[] disponivel = new int[TOTAL_RECURSOS];

    int[,] maximo = new int[TOTAL_CLIENTES, TOTAL_RECURSOS];

    int[,] alocado = new int[TOTAL_CLIENTES, TOTAL_RECURSOS];

    int[,] necessidade = new int[TOTAL_CLIENTES, TOTAL_RECURSOS];

    object trava = new object();

    bool estadoSeguro(int cliente, int[] pedido)
    {
        int[] disponivelTemp = new int[TOTAL_RECURSOS];
        int[,] alocadoTemp = new int[TOTAL_CLIENTES, TOTAL_RECURSOS];
        int[,] necessidadeTemp = new int[TOTAL_CLIENTES, TOTAL_RECURSOS];

        for (int r = 0; r < TOTAL_RECURSOS; r++)
            disponivelTemp[r] = disponivel[r];

        for (int c = 0; c < TOTAL_CLIENTES; c++)
            for (int r = 0; r < TOTAL_RECURSOS; r++)
            {
                alocadoTemp[c, r] = alocado[c, r];
                necessidadeTemp[c, r] = necessidade[c, r];
            }

        for (int r = 0; r < TOTAL_RECURSOS; r++)
        {
            disponivelTemp[r] -= pedido[r];
            alocadoTemp[cliente, r] += pedido[r];
            necessidadeTemp[cliente, r] -= pedido[r];
        }

        bool[] terminou = new bool[TOTAL_CLIENTES];
        bool achouAlguem;

        do
        {
            achouAlguem = false;

            for (int c = 0; c < TOTAL_CLIENTES; c++)
            {
                if (terminou[c]) continue;

                bool podeTerminar = true;
                for (int r = 0; r < TOTAL_RECURSOS; r++)
                {
                    if (necessidadeTemp[c, r] > disponivelTemp[r])
                    {
                        podeTerminar = false;
                        break;
                    }
                }

                if (podeTerminar)
                {
                    for (int r = 0; r < TOTAL_RECURSOS; r++)
                        disponivelTemp[r] += alocadoTemp[c, r];

                    terminou[c] = true;
                    achouAlguem = true;
                }
            }

        } while (achouAlguem);

        for (int c = 0; c < TOTAL_CLIENTES; c++)
            if (!terminou[c]) return false;

        return true;
    }

    public void SetDisponivel(int[] recursos)
    {
        for (int r = 0; r < TOTAL_RECURSOS; r++)
            disponivel[r] = recursos[r];
    }

    public void SetMaximo(int cliente, int[] max)
    {
        for (int r = 0; r < TOTAL_RECURSOS; r++)
            maximo[cliente, r] = max[r];
    }

    public void RecalculaNecessidade()
    {
        for (int c = 0; c < TOTAL_CLIENTES; c++)
            for (int r = 0; r < TOTAL_RECURSOS; r++)
                necessidade[c, r] = maximo[c, r] - alocado[c, r];
    }

    public int SolicitarRecursos(int cliente, int[] pedido)
    {
        lock (trava)
        {
            for (int r = 0; r < TOTAL_RECURSOS; r++)
            {
                if (pedido[r] > necessidade[cliente, r])
                {
                    Console.WriteLine($"[Cliente {cliente}] Pedido negado: excede necessidade.");
                    return -1;
                }

                if (pedido[r] > disponivel[r])
                {
                    Console.WriteLine($"[Cliente {cliente}] Pedido negado: recursos insuficientes.");
                    return -1;
                }
            }

            if (!estadoSeguro(cliente, pedido))
            {
                Console.WriteLine($"[Cliente {cliente}] Pedido negado: estado inseguro.");
                return -1;
            }

            for (int r = 0; r < TOTAL_RECURSOS; r++)
            {
                disponivel[r] -= pedido[r];
                alocado[cliente, r] += pedido[r];
            }

            RecalculaNecessidade();

            Console.WriteLine($"[Cliente {cliente}] Recursos alocados com sucesso.");
            return 0;
        }
    }

    public int LiberarRecursos(int cliente, int[] liberacao)
    {
        lock (trava)
        {
            for (int r = 0; r < TOTAL_RECURSOS; r++)
            {
                if (liberacao[r] > alocado[cliente, r])
                {
                    Console.WriteLine($"[Cliente {cliente}] Erro ao liberar: não tem tantos recursos.");
                    return -1;
                }
            }

            for (int r = 0; r < TOTAL_RECURSOS; r++)
            {
                disponivel[r] += liberacao[r];
                alocado[cliente, r] -= liberacao[r];
            }

            RecalculaNecessidade();

            Console.WriteLine($"[Cliente {cliente}] Recursos liberados.");
            return 0;
        }
    }

    public void LoopDoCliente(int cliente)
    {
        Random rng = new Random();

        while (true)
        {
            int[] pedido = new int[TOTAL_RECURSOS];
            for (int r = 0; r < TOTAL_RECURSOS; r++)
            {
                int maxPedido = necessidade[cliente, r];
                pedido[r] = maxPedido > 0 ? rng.Next(0, maxPedido + 1) : 0;
            }

            int resultado = SolicitarRecursos(cliente, pedido);

            if (resultado == 0)
            {
                Thread.Sleep(rng.Next(500, 1500));

                LiberarRecursos(cliente, pedido);
            }

            Thread.Sleep(rng.Next(200, 800));
        }
    }

    public int GetTotalClientes() => TOTAL_CLIENTES;
    public int GetTotalRecursos() => TOTAL_RECURSOS;
}

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Uso: dotnet run <recurso1> <recurso2> <recurso3>");
            Console.WriteLine("Exemplo: dotnet run 10 5 7");
            return;
        }

        Banqueiro banqueiro = new Banqueiro();

        int[] recursos = new int[args.Length];
        for (int i = 0; i < args.Length; i++)
            recursos[i] = int.Parse(args[i]);

        banqueiro.SetDisponivel(recursos);

        Console.WriteLine("=== Algoritmo do Banqueiro ===");
        Console.WriteLine($"Recursos iniciais: [{string.Join(", ", recursos)}]");
        Console.WriteLine($"Clientes: {banqueiro.GetTotalClientes()}");
        Console.WriteLine("==============================\n");

        Random rng = new Random();

        for (int c = 0; c < banqueiro.GetTotalClientes(); c++)
        {
            int[] max = new int[banqueiro.GetTotalRecursos()];
            for (int r = 0; r < banqueiro.GetTotalRecursos(); r++)
                max[r] = rng.Next(1, recursos[r % recursos.Length] + 1);

            banqueiro.SetMaximo(c, max);
        }

        banqueiro.RecalculaNecessidade();

        for (int c = 0; c < banqueiro.GetTotalClientes(); c++)
        {
            int id = c;
            Thread t = new Thread(() => banqueiro.LoopDoCliente(id));
            t.IsBackground = true;
            t.Start();
        }

        Console.WriteLine("Todas as threads iniciadas. Pressione Enter para encerrar.\n");
        Console.ReadLine();
    }
}
