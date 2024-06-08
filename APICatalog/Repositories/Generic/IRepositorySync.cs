using System.Linq.Expressions;

namespace APICatalog.Repositories.Generic;

public interface IRepositorySync<T>
{
    IEnumerable<T?> GetAll();
    IQueryable<T?> GetAllQueryable();
    T? GetById(int id);
    T? Create(T? entity);
    T? Update(T? entity);
    T? DeleteById(int id);
    bool Exists(int id);
    int Count();
    IEnumerable<T?> Find(Expression<Func<T?, bool>> predicate);
    T? FirstOrDefault(Expression<Func<T?, bool>> predicate);
}
/*
     IEnumerable<T>:

        Execução Imediata: Quando você chama GetAllEnumerable(), a consulta ao banco de dados é executada imediatamente,
            e todos os registros correspondentes são carregados na memória como uma coleção IEnumerable<T>.

        Filtragem na Memória: Qualquer filtragem ou ordenação adicional que você aplicar a essa coleção será feita na memória,
            após os dados já terem sido recuperados do banco de dados.

        Cenários de Uso: Ideal para quando você precisa trabalhar com uma coleção de dados relativamente pequena
            que cabe na memória e não requer filtragem complexa no lado do servidor.
---------------------------------------------------------------------------------------------------------------------------------
        IQueryable<T>:

        Execução Diferida: GetAllQueryable() não executa a consulta ao banco de dados imediatamente.
            Em vez disso, ele retorna um objeto IQueryable<T> que representa a consulta.

        Construção de Consultas: Você pode usar métodos de extensão LINQ (como Where, OrderBy, Skip, Take)
            para construir uma consulta complexa em cima do IQueryable<T>.

        Filtragem no Banco de Dados: Quando você finalmente enumera o IQueryable<T> (por exemplo, usando ToList()),
            a consulta construída é traduzida em SQL e executada no banco de dados.
            Isso significa que a filtragem e a ordenação ocorrem no lado do servidor,
            o que geralmente é mais eficiente para grandes conjuntos de dados.

        Cenários de Uso: Ideal para quando você precisa aplicar filtros, ordenação ou
            paginação complexos no lado do servidor antes de recuperar os dados.
---------------------------------------------------------------------------------------------------------------------------
     */