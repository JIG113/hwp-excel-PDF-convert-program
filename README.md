# HWP/Office to PDF Converter (Local Desktop)

로컬 컴퓨터에 설치해 사용하는 문서 PDF 변환기 MVP 프로젝트입니다.

## 목표
- HWP, DOCX, XLSX, PPTX, TXT, JPG/PNG를 PDF로 변환
- 웹사이트가 아닌 로컬 설치형 데스크톱 애플리케이션
- 배치 변환, 로그, 실패 리포트 제공

## 아키텍처(초기)
- `DesktopApp`: UI 셸 및 작업 제어
<<<<<<< codex/develop-local-pdf-conversion-tool-9cj298
- `Core`: 변환 요청/결과 모델, 라우터, 작업 큐, 실행 커맨드 러너
- `Adapters`: 포맷별 변환 어댑터(HWP/Office/TextImage)

## 현재 상태
초기 구현 완료:
1. 공통 모델/인터페이스
2. 확장자 기반 라우터
3. 메모리 기반 작업 큐(JobManager)
4. 변환 전 경로 검증(Preflight)
5. Office 어댑터의 LibreOffice headless 실행 뼈대
6. Text/Image 어댑터의 임시 성공 경로(파일 처리 스텁)

## 다음 단계
- .NET Desktop UI 프로젝트 생성 및 Core 연결
- Text/Image 실제 PDF 생성 엔진 연결
- HWP 중간 포맷 파이프라인 연결
- 로그/실패 리포트 저장 구현
=======
- `Core`: 변환 요청/결과 모델, 라우터, 작업 큐
- `Adapters`: 포맷별 변환 어댑터(HWP/Office/TextImage)

## 현재 상태
초기 스캐폴딩 완료:
1. 공통 모델/인터페이스
2. 확장자 기반 라우터
3. 메모리 기반 작업 큐(JobManager)
4. 어댑터 스텁 구현

다음 단계:
- .NET UI 프로젝트 생성 및 Core 연결
- LibreOffice 연동(Office 어댑터)
- HWP 중간 포맷 파이프라인 연결
>>>>>>> main
