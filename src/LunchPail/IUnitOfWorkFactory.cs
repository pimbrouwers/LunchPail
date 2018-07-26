using System;

namespace LunchPail
{
  public interface IUnitOfWorkFactory
  {
    IUnitOfWork Create();
  }
}