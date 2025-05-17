using System;

namespace PersonalFinanceTrackerAPI.Models;

public class PaginatedList<T>
{
  public List<T> Items { get; private set; } // Elementos de la página actual.
  public int TotalCount { get; private set; } // Total de elementos 
  public int PageSize { get; private set; } // Tamaño de la página.
  public int CurrentPage { get; private set; } // Página actual.

  public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
  {
    Items = items;
    TotalCount = count;
    CurrentPage = pageIndex < 1 ? 1 : pageIndex;
    PageSize = pageSize < 0 ? 0 : pageSize;
  }

  // Total de páginas, calculado dinámicamente
  public int TotalPages => PageSize > 0 
    ? (int)Math.Ceiling(TotalCount / (double)PageSize) 
    : (TotalCount > 0 ? 1 : 0); // Si hay elementos pero PageSize es 0, devuelve 1 página

  public bool HasPreviousPage => CurrentPage > 1;
  public bool HasNextPage => CurrentPage < TotalPages;
}

