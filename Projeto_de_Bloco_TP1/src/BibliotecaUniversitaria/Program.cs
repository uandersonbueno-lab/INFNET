// =====================================================================
// Sistema de Gestão de Biblioteca Universitária — programa standalone (TP1)
// Aluno: Uanderson Bueno — Disciplina: 26E4_5 Projeto de Bloco
// =====================================================================

using System.Globalization;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

// ---------- Parâmetros das regras de negócio (RNF11) ----------
const int PrazoDias = 14;            // RN01: prazo padrão de empréstimo
const int MaxEmprestimos = 3;        // RN03: empréstimos simultâneos por membro
const decimal MultaPorDia = 1.00m;   // Suposição S01: valor da multa por dia de atraso
const int MaxReservas = 50;          // capacidade do vetor de reservas

// Estados de conservação do exemplar (Suposição S11)
const int EstadoBom = 0;
const int EstadoDanificado = 1;
const int EstadoIndisponivel = 2;

const int Nenhum = -1;               // indica "sem membro" / "não encontrado"

DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

// ---------- Livros (dados de exemplo; ISBNs fictícios) ----------
string[] livroTitulo = { "Código Limpo", "Algoritmos: Teoria e Prática", "Engenharia de Software", "Introdução à Programação em C#" };
string[] livroAutor = { "Robert C. Martin", "Thomas H. Cormen", "Ian Sommerville", "Maria Oliveira" };
string[] livroIsbn = { "978-85-0001-001-1", "978-85-0001-002-8", "978-85-0001-003-5", "978-85-0001-004-2" };

// ---------- Exemplares ----------
string[] exPatrimonio = { "PAT-001", "PAT-002", "PAT-003", "PAT-004", "PAT-005", "PAT-006", "PAT-007" };
int[] exLivro = { 0, 0, 1, 1, 2, 2, 3 };
int[] exEstado = { EstadoBom, EstadoBom, EstadoBom, EstadoDanificado, EstadoBom, EstadoIndisponivel, EstadoBom };
int[] exMembro = { Nenhum, Nenhum, Nenhum, Nenhum, Nenhum, Nenhum, Nenhum }; // membro com empréstimo ativo
DateOnly[] exRetirada = new DateOnly[exPatrimonio.Length];
DateOnly[] exPrazo = new DateOnly[exPatrimonio.Length];

// ---------- Membros ----------
string[] membroNome = { "Ana Lima", "Bruno Costa", "Carla Dias", "Diego Rocha" };
string[] membroMatricula = { "2026001", "2026002", "2026003", "2026004" };
string[] membroTipo = { "Aluno", "Professor", "Funcionário", "Aluno" };
bool[] membroAtivo = { true, true, false, true };           // cadastro válido (Suposição S07)
int[] membroQtdEmprestimos = { 0, 0, 0, 0 };
decimal[] membroMultaPendente = { 0m, 3.00m, 0m, 0m };     // Bruno começa com multa pendente

// ---------- Reservas ----------
int[] reservaLivro = new int[MaxReservas];
int[] reservaMembro = new int[MaxReservas];
DateOnly[] reservaData = new DateOnly[MaxReservas];
int qtdReservas = 0;

// Situação inicial para demonstração: Ana pegou o PAT-003 há 20 dias (está 6 dias atrasada).
exMembro[2] = 0;
exRetirada[2] = hoje.AddDays(-20);
exPrazo[2] = exRetirada[2].AddDays(PrazoDias);
membroQtdEmprestimos[0] = 1;

// =====================================================================
// Laço principal do menu
// =====================================================================
bool executando = true;
while (executando)
{
    Console.WriteLine();
    Console.WriteLine("==============================================");
    Console.WriteLine("   BIBLIOTECA UNIVERSITÁRIA — " + hoje.ToString("dd/MM/yyyy"));
    Console.WriteLine("==============================================");
    Console.WriteLine(" 1 - Consultar acervo e disponibilidade");
    Console.WriteLine(" 2 - Realizar empréstimo");
    Console.WriteLine(" 3 - Registrar devolução");
    Console.WriteLine(" 4 - Realizar reserva");
    Console.WriteLine(" 5 - Consultar membros, empréstimos e multas");
    Console.WriteLine(" 6 - Registrar pagamento de multa");
    Console.WriteLine(" 0 - Sair");

    int opcao = LerInteiro("Escolha uma opção: ", 0, 6);
    Console.WriteLine();

    switch (opcao)
    {
        case 1: ConsultarAcervo(); break;
        case 2: RealizarEmprestimo(); break;
        case 3: RegistrarDevolucao(); break;
        case 4: RealizarReserva(); break;
        case 5: ConsultarMembros(); break;
        case 6: PagarMulta(); break;
        case 0: executando = false; break;
    }
}
Console.WriteLine("Programa encerrado.");

// =====================================================================
// UC01 — Consultar acervo e disponibilidade (RF11, RF12)
// =====================================================================
void ConsultarAcervo()
{
    Console.Write("Buscar por título, autor ou ISBN (Enter = todos): ");
    string termo = (Console.ReadLine() ?? "").Trim().ToLower();
    int encontrados = 0;

    for (int l = 0; l < livroTitulo.Length; l++)
    {
        bool corresponde = termo == ""
            || livroTitulo[l].ToLower().Contains(termo)
            || livroAutor[l].ToLower().Contains(termo)
            || livroIsbn[l].Contains(termo);
        if (!corresponde)
        {
            continue;
        }

        encontrados++;
        Console.WriteLine($"[{l + 1}] {livroTitulo[l]} — {livroAutor[l]} (ISBN {livroIsbn[l]})");
        Console.WriteLine($"    Disponíveis: {ContarDisponiveis(l)} | Emprestados: {ContarEmprestados(l)} | Indisponíveis/danificados: {ContarForaDeUso(l)}");

        for (int e = 0; e < exPatrimonio.Length; e++)
        {
            if (exLivro[e] == l)
            {
                Console.WriteLine($"      {exPatrimonio[e]}: {DescreverSituacao(e)}");
            }
        }
    }

    if (encontrados == 0)
    {
        Aviso("Nenhum livro encontrado.");
    }
}

// =====================================================================
// UC02 — Realizar empréstimo (RF14–RF19; RN01, RN03, RN04, RN05, RN10, RN12)
// =====================================================================
void RealizarEmprestimo()
{
    int m = BuscarMembro(LerTexto("Matrícula do membro: "));

    // UC08 — validações do membro
    if (m == Nenhum || !membroAtivo[m])
    {
        Aviso("Empréstimo recusado: membro sem cadastro válido (RN12).");
        return;
    }
    if (membroQtdEmprestimos[m] >= MaxEmprestimos)
    {
        Aviso($"Empréstimo recusado: {membroNome[m]} já possui {MaxEmprestimos} empréstimos ativos (RN03).");
        ListarEmprestimosDoMembro(m);
        return;
    }
    if (membroMultaPendente[m] > 0)
    {
        Aviso($"Empréstimo recusado: {membroNome[m]} possui multa pendente de {membroMultaPendente[m]:C} (Suposição S03).");
        return;
    }

    int e = BuscarExemplar(LerTexto("Código de patrimônio do exemplar: "));

    // UC08 — validações do exemplar
    if (e == Nenhum)
    {
        Aviso("Exemplar não encontrado.");
        return;
    }
    if (exMembro[e] != Nenhum || exEstado[e] != EstadoBom)
    {
        Aviso($"Empréstimo recusado: {exPatrimonio[e]} está {DescreverSituacao(e)} (RN04, RN05, RN10).");
        int alternativa = BuscarExemplarDisponivel(exLivro[e]);
        if (alternativa != Nenhum)
        {
            Console.WriteLine($"  Sugestão: o exemplar {exPatrimonio[alternativa]} do mesmo livro está disponível.");
        }
        else
        {
            Console.WriteLine("  Não há outro exemplar disponível deste livro. O membro pode fazer uma reserva (opção 4).");
        }
        return;
    }

    DateOnly retirada = LerData("Data de retirada (dd/MM/aaaa, Enter = hoje): ");
    exMembro[e] = m;
    exRetirada[e] = retirada;
    exPrazo[e] = retirada.AddDays(PrazoDias);   // RN01
    membroQtdEmprestimos[m]++;

    Sucesso("Empréstimo registrado!");
    Console.WriteLine($"  Livro:     {livroTitulo[exLivro[e]]} ({exPatrimonio[e]})");
    Console.WriteLine($"  Membro:    {membroNome[m]} ({membroTipo[m]})");
    Console.WriteLine($"  Retirada:  {retirada:dd/MM/yyyy}");
    Console.WriteLine($"  Devolver até: {exPrazo[e]:dd/MM/yyyy}");
    Console.WriteLine($"  Empréstimos ativos do membro: {membroQtdEmprestimos[m]} de {MaxEmprestimos}");
}

// =====================================================================
// UC03 — Registrar devolução (RF20–RF22, RF29; RN02, RN08, RN09)
// =====================================================================
void RegistrarDevolucao()
{
    int e = BuscarExemplar(LerTexto("Código de patrimônio do exemplar: "));
    if (e == Nenhum)
    {
        Aviso("Exemplar não encontrado.");
        return;
    }
    if (exMembro[e] == Nenhum)
    {
        Aviso("Não há empréstimo ativo para este exemplar.");
        return;
    }

    DateOnly devolucao = LerData("Data da devolução (dd/MM/aaaa, Enter = hoje): ");
    if (devolucao < exRetirada[e])
    {
        Aviso($"Data inválida: a devolução não pode ser antes da retirada ({exRetirada[e]:dd/MM/yyyy}).");
        return;
    }

    int m = exMembro[e];
    int diasAtraso = devolucao.DayNumber - exPrazo[e].DayNumber;  // dias corridos (S02, S08)

    // RN08: encerra o empréstimo
    exMembro[e] = Nenhum;
    membroQtdEmprestimos[m]--;
    Sucesso($"Devolução de {exPatrimonio[e]} registrada em {devolucao:dd/MM/yyyy} (prazo: {exPrazo[e]:dd/MM/yyyy}).");

    // UC09 — Calcular multa por atraso (extend: só quando há atraso)
    if (diasAtraso > 0)
    {
        decimal multa = diasAtraso * MultaPorDia;
        membroMultaPendente[m] += multa;
        Aviso($"Devolução com {diasAtraso} dia(s) de atraso. Multa de {multa:C} gerada para {membroNome[m]} (RN09).");
    }
    else
    {
        Console.WriteLine("  Devolução dentro do prazo. Nenhuma multa gerada.");
    }

    // RF22 / FA3: exemplar danificado não volta a circular
    if (LerSimNao("O exemplar foi devolvido danificado? (s/n): "))
    {
        exEstado[e] = EstadoDanificado;
        Aviso("Exemplar marcado como danificado. Ele só poderá ser emprestado após regularização (RN10).");
    }

    // RF29: avisar a reserva mais antiga do livro
    int r = BuscarPrimeiraReserva(exLivro[e]);
    if (r != Nenhum)
    {
        Console.WriteLine($"  Atenção: há reserva para este livro. Próximo da fila: {membroNome[reservaMembro[r]]} (desde {reservaData[r]:dd/MM/yyyy}).");
    }
}

// =====================================================================
// UC04 — Realizar reserva (RF25–RF27; RN06, RN07, RN12)
// =====================================================================
void RealizarReserva()
{
    int m = BuscarMembro(LerTexto("Matrícula do membro: "));
    if (m == Nenhum || !membroAtivo[m])
    {
        Aviso("Reserva recusada: membro sem cadastro válido (RN12).");
        return;
    }

    for (int l = 0; l < livroTitulo.Length; l++)
    {
        Console.WriteLine($"  [{l + 1}] {livroTitulo[l]} — disponíveis: {ContarDisponiveis(l)}, emprestados: {ContarEmprestados(l)}");
    }
    int livro = LerInteiro("Número do livro: ", 1, livroTitulo.Length) - 1;

    // UC10 — Verificar exemplares emprestados (RN06)
    if (ContarEmprestados(livro) == 0)
    {
        Aviso("Reserva não permitida: nenhum exemplar deste livro está emprestado (RN06).");
        return;
    }

    // Suposição S05: uma reserva por membro e livro
    for (int r = 0; r < qtdReservas; r++)
    {
        if (reservaLivro[r] == livro && reservaMembro[r] == m)
        {
            Aviso($"{membroNome[m]} já possui reserva para este livro (posição {PosicaoNaFila(r)} na fila).");
            return;
        }
    }

    if (qtdReservas == MaxReservas)
    {
        Aviso("Limite de reservas do sistema atingido.");
        return;
    }

    // RN07: a reserva guarda o LIVRO, não um exemplar
    reservaLivro[qtdReservas] = livro;
    reservaMembro[qtdReservas] = m;
    reservaData[qtdReservas] = hoje;
    qtdReservas++;

    Sucesso($"Reserva de \"{livroTitulo[livro]}\" registrada para {membroNome[m]} em {hoje:dd/MM/yyyy}.");
    Console.WriteLine($"  Posição na fila: {PosicaoNaFila(qtdReservas - 1)}");
}

// =====================================================================
// Consultas e multas (RF13, RF23, RF24)
// =====================================================================
void ConsultarMembros()
{
    for (int m = 0; m < membroNome.Length; m++)
    {
        string situacao = membroAtivo[m] ? "ativo" : "INATIVO";
        Console.WriteLine($"{membroMatricula[m]} - {membroNome[m]} ({membroTipo[m]}, cadastro {situacao})");
        Console.WriteLine($"    Empréstimos ativos: {membroQtdEmprestimos[m]}/{MaxEmprestimos} | Multa pendente: {membroMultaPendente[m]:C}");
        ListarEmprestimosDoMembro(m);
    }
}

void PagarMulta()
{
    int m = BuscarMembro(LerTexto("Matrícula do membro: "));
    if (m == Nenhum)
    {
        Aviso("Membro não encontrado.");
        return;
    }
    if (membroMultaPendente[m] == 0)
    {
        Console.WriteLine($"  {membroNome[m]} não possui multas pendentes.");
        return;
    }
    if (LerSimNao($"Confirmar pagamento de {membroMultaPendente[m]:C} por {membroNome[m]}? (s/n): "))
    {
        membroMultaPendente[m] = 0;
        Sucesso("Pagamento registrado. Situação da multa: paga.");
    }
}

void ListarEmprestimosDoMembro(int m)
{
    for (int e = 0; e < exPatrimonio.Length; e++)
    {
        if (exMembro[e] == m)
        {
            string atraso = hoje > exPrazo[e] ? $" — ATRASADO {hoje.DayNumber - exPrazo[e].DayNumber} dia(s)" : "";
            Console.WriteLine($"      {exPatrimonio[e]} {livroTitulo[exLivro[e]]}, devolver até {exPrazo[e]:dd/MM/yyyy}{atraso}");
        }
    }
}

// =====================================================================
// Funções auxiliares de busca e contagem
// =====================================================================
int BuscarMembro(string matricula)
{
    for (int m = 0; m < membroMatricula.Length; m++)
    {
        if (membroMatricula[m] == matricula) return m;
    }
    return Nenhum;
}

int BuscarExemplar(string patrimonio)
{
    for (int e = 0; e < exPatrimonio.Length; e++)
    {
        if (exPatrimonio[e].Equals(patrimonio, StringComparison.OrdinalIgnoreCase)) return e;
    }
    return Nenhum;
}

bool EstaDisponivel(int e) => exMembro[e] == Nenhum && exEstado[e] == EstadoBom;

int BuscarExemplarDisponivel(int livro)
{
    for (int e = 0; e < exPatrimonio.Length; e++)
    {
        if (exLivro[e] == livro && EstaDisponivel(e)) return e;
    }
    return Nenhum;
}

int ContarDisponiveis(int livro)
{
    int total = 0;
    for (int e = 0; e < exPatrimonio.Length; e++)
    {
        if (exLivro[e] == livro && EstaDisponivel(e)) total++;
    }
    return total;
}

int ContarEmprestados(int livro)
{
    int total = 0;
    for (int e = 0; e < exPatrimonio.Length; e++)
    {
        if (exLivro[e] == livro && exMembro[e] != Nenhum) total++;
    }
    return total;
}

int ContarForaDeUso(int livro)
{
    int total = 0;
    for (int e = 0; e < exPatrimonio.Length; e++)
    {
        if (exLivro[e] == livro && exMembro[e] == Nenhum && exEstado[e] != EstadoBom) total++;
    }
    return total;
}

string DescreverSituacao(int e)
{
    if (exMembro[e] != Nenhum)
    {
        return $"emprestado para {membroNome[exMembro[e]]} até {exPrazo[e]:dd/MM/yyyy}";
    }
    switch (exEstado[e])
    {
        case EstadoBom: return "disponível";
        case EstadoDanificado: return "danificado";
        default: return "indisponível";
    }
}

int BuscarPrimeiraReserva(int livro)
{
    // As reservas são gravadas em ordem de data, então a primeira encontrada é a mais antiga (S06).
    for (int r = 0; r < qtdReservas; r++)
    {
        if (reservaLivro[r] == livro) return r;
    }
    return Nenhum;
}

int PosicaoNaFila(int indiceReserva)
{
    int posicao = 0;
    for (int r = 0; r <= indiceReserva; r++)
    {
        if (reservaLivro[r] == reservaLivro[indiceReserva]) posicao++;
    }
    return posicao;
}

// =====================================================================
// Funções de entrada e saída (RNF05–RNF08)
// =====================================================================
int LerInteiro(string mensagem, int minimo, int maximo)
{
    while (true)
    {
        Console.Write(mensagem);
        string texto = Console.ReadLine() ?? "";
        if (int.TryParse(texto, out int valor) && valor >= minimo && valor <= maximo)
        {
            return valor;
        }
        Aviso($"Entrada inválida. Digite um número entre {minimo} e {maximo}.");
    }
}

string LerTexto(string mensagem)
{
    Console.Write(mensagem);
    return (Console.ReadLine() ?? "").Trim();
}

DateOnly LerData(string mensagem)
{
    while (true)
    {
        Console.Write(mensagem);
        string texto = (Console.ReadLine() ?? "").Trim();
        if (texto == "")
        {
            return hoje;
        }
        if (DateOnly.TryParseExact(texto, "dd/MM/yyyy", out DateOnly data))
        {
            return data;
        }
        Aviso("Data inválida. Use o formato dd/MM/aaaa.");
    }
}

bool LerSimNao(string mensagem)
{
    while (true)
    {
        Console.Write(mensagem);
        string resposta = (Console.ReadLine() ?? "").Trim().ToLower();
        if (resposta == "s") return true;
        if (resposta == "n") return false;
        Aviso("Responda com s ou n.");
    }
}

void Aviso(string mensagem)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("  " + mensagem);
    Console.ResetColor();
}

void Sucesso(string mensagem)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("  " + mensagem);
    Console.ResetColor();
}
