using System.ComponentModel.DataAnnotations;

namespace ControlApi.API.DTOs;

public record PullAsyncRequestDto(bool overrideLogic, DateTime dateOverride, bool rePullOldItems);
public record PullAsyncResponseDto(bool completed);
