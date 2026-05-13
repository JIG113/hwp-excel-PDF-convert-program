using System.Collections.Generic;

namespace Converter.Core.Conversion;

public static class ConversionErrorMessages
{
    private static readonly IReadOnlyDictionary<ConversionErrorCode, string> Messages =
        new Dictionary<ConversionErrorCode, string>
        {
            [ConversionErrorCode.EngineNotFound] = "변환 엔진을 찾을 수 없습니다.",
            [ConversionErrorCode.UnsupportedFormat] = "지원하지 않는 파일 형식입니다.",
            [ConversionErrorCode.InputFileMissing] = "입력 파일이 존재하지 않습니다.",
            [ConversionErrorCode.OutputDirectoryMissing] = "출력 폴더가 존재하지 않습니다.",
            [ConversionErrorCode.Timeout] = "변환 시간이 제한을 초과했습니다.",
            [ConversionErrorCode.ProcessCrash] = "변환 엔진이 비정상 종료되었습니다.",
            [ConversionErrorCode.OutputMissing] = "출력 PDF가 생성되지 않았습니다.",
            [ConversionErrorCode.PermissionDenied] = "파일 접근 권한이 없습니다.",
            [ConversionErrorCode.ConversionFailed] = "문서 변환에 실패했습니다.",
        };

    public static string Resolve(ConversionErrorCode code)
        => Messages.TryGetValue(code, out var message)
            ? message
            : "알 수 없는 오류가 발생했습니다.";
}
