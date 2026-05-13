# HWP 변환 전략

## 옵션 비교
| 전략 | 품질 | 속도 | 라이선스 | 안정성 | 구현 난이도 |
|---|---|---|---|---|---|
| LibreOffice 기반 | 중 | 중 | LGPL | 중 | 중 |
| 한컴오피스 COM 자동화 | 높음(Windows) | 중 | Office/한컴 라이선스 필요 | 중~높음 | 높음 |
| 외부 상용 엔진 | 높음 | 높음 | 상용 비용 | 높음 | 중 |
| HWPX 우선 전략(XML) | 중~높음(구조 보존 시) | 중 | 엔진에 따라 상이 | 중 | 중 |

## 권장 아키텍처
1. **HWPX -> LibreOffice** 우선 변환
2. **HWP -> 한컴 COM** 경로 보강
3. 실패 시 fallback converter 체인 적용

## 제안 구조
- `IHwpConverter` 인터페이스
- `HwpxLibreOfficeConverter`
- `HwpComConverter`
- `FallbackHwpConverter`(Chain of Responsibility)
