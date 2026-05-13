# HWP/Office to PDF Converter (Local Desktop)

로컬 설치형 문서→PDF 변환기 프로젝트입니다.

## Requirements
- .NET SDK **8.0.x**
- Windows 10/11 (WPF UI 실행)
- LibreOffice (Office 변환 품질 검증 시)

## 개발환경 실행 방법
```bash
dotnet restore hwp-excel-PDF-convert-program.sln
dotnet build hwp-excel-PDF-convert-program.sln
dotnet test hwp-excel-PDF-convert-program.sln
```

## 프로젝트 구성
- `src/Core`: 변환 모델, 라우팅, 작업 큐
- `src/Adapters`: Office/HWP/TextImage 어댑터
- `src/DesktopApp`: WPF(MVVM) UI
- `tests/*`: 단위/통합 테스트

## 현재 구현 상태
- ConverterRouter / ConversionPreflight / ProcessCommandRunner / JobManager
- OfficeConverter 안정화(엔진 경로 탐색, timeout, output 검증)
- 기본 WPF UI + Job Queue 표시 + JobManager 연결
- GitHub Actions Windows CI(build/test)
