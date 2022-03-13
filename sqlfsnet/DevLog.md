# 개발 기록

### 2022-03-13

고전적인 directory 방식이 아닌 다층의 여러 구조에 포함될 수 있는 파일 구조를 생각.

```csharp
    public class File
    public class Tag // directory like (grouping) - / 로시작하는 이름을 고전적인 directory 로 취급?
    internal class FileContent
    internal class FileOnTag // File-Tag list
```

이렇게 예상한 구조에서는 경로내 파일 중복을 확인하기 까다롭다. google drive 처럼 중복 파일이름을
허용하는것이 현실적일 것. 

Tag 내에 full 경로를 이용하는 방식을 생각해보았으나

  * 하위 경로가 많아질수록 (`/a/b/c/d/e/...`) 이름이 차지하는 데이터 공간이 많아져 비효율적 (minor)
  * 전체이름이 아닌 directory 접근과 같은 부분 경로를 검색하기가 까다로움 (major)

와 같은 문제. 따라서 [기존 filesystem 과 유사](https://www.codeproject.com/Articles/336112/Simple-file-system-over-SQLite)
한 방식으로 우선 개발 후에 grouping 을 지원하는 확장을 고려하는편이 현실적일 것.

