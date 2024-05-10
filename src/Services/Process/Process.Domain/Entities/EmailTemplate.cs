using System.ComponentModel.DataAnnotations;

namespace Ecmanage.eProcessor.Services.Process.Process.Domain.Entities;

public abstract class EmailTemplate : BaseEntity
{
  public EmailTemplate()
  {
  }

  public EmailQueueItem EmailQueueItem { get; set; } = null!;
}