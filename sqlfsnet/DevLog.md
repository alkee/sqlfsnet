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


### 2022-03-17

비동기로 [in-memory db](https://www.sqlite.org/inmemorydb.html)이용해 테스트하는 경우
sqlite 공식 스펙(각각 independent 한 memory instance)과 [다른 동작](https://github.com/praeclarum/sqlite-net/issues/403).
slqite-net 에서 모두 connection 이 공유되는 memory db 로 접근이 되는 듯.
그래서 비동기로 구성된 테스트에서 이미 존재하는 데이터 때문에 같은이름의 데이터를 사용하는 경우 오류 발생.

아래와 같이 uri 를 이용하는 방식으로도 마찬가지. (`file` 이라는 빈 파일 생성)

```csharp
    static int dbCount = 0;
    public static sqlfsnet.FileSystem CreateFileSystem()
    {
        var dbPath = $"file:memdb{dbCount++}?mode=memory"; // https://github.com/praeclarum/sqlite-net/issues/1077
        return new sqlfsnet.FileSystem(dbPath);
    }
```

강제로 URI 를 지원할 수 있도록 [openflag(SQLITE_OPEN_URI)](https://www.sqlite.org/c3ref/c_open_autoproxy.html) 추가.
uri 형태의 `file::memory:` 로 동작은 하지만, 마찬가지로 connection 간에 공유된 database 를 사용하는것으로 보임.
따라서, 위와같은 counting 형태의 database name 을 사용하는것으로..

