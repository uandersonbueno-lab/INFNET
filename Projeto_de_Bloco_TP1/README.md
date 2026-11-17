# Sistema de Gestão de Biblioteca Universitária

- **Aluno:** Uanderson Bueno
- **Disciplina:** 26E4_5 — Projeto de Bloco
- **Entrega:** TP1
- **Trilha:** C# / .NET 10 (LTS)

## Estrutura do repositório

```
docs/
  requisitos.md                 Requisitos funcionais, não funcionais e regras de negócio
  casos-de-uso.md               Especificação dos casos de uso (fluxos principal e alternativos)
  diagrama-casos-de-uso.svg     Diagrama de casos de uso (UML)
  diagrama-casos-de-uso.puml    Fonte do diagrama em PlantUML
  justificativa-escopo.md       Escopo do TP1, o que ficou para depois e suposições
  Uanderson_Bueno_26E4_5_PB_TP1.pdf   Documento consolidado para entrega
src/
  BibliotecaUniversitaria/      Programa standalone inicial (console .NET 10, sem OO)
```

## Como executar

Pré-requisito: .NET 10 SDK.

```
dotnet run --project src/BibliotecaUniversitaria
```

Ou abra `BibliotecaUniversitaria.slnx` no Visual Studio Community e pressione F5.

### Dados de exemplo para testar

| Matrícula | Membro | Situação inicial |
|-----------|--------|------------------|
| 2026001 | Ana Lima (aluna) | Está com o PAT-003 há 20 dias, ou seja, 6 dias atrasada |
| 2026002 | Bruno Costa (professor) | Tem multa pendente de R$ 3,00 e não consegue pegar livros |
| 2026003 | Carla Dias (funcionária) | Cadastro inativo |
| 2026004 | Diego Rocha (aluno) | Sem pendências |

Exemplares: PAT-001 a PAT-007. O PAT-004 está danificado e o PAT-006 está indisponível.

## Uso de ferramentas de IA

**Ferramenta:** Claude Code (Anthropic), modelo Claude Opus 5.5.
 A ferramenta foi usada para ajustes e correções, também foi usada para auxilio nas soluções e correção de texto e relatorios. Todo o conteúdo foi revisado e validado pelo aluno.
