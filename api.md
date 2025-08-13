# InfoCenterController.RegisterResidentDevice API 文件
# 準備訊息中心位址，例如：http://192.168.68.249/MessageCenter。
## API 路徑

```
POST /InfoCenter/RegisterResidentDevice
```

## 說明

此 API 用於註冊住戶裝置，驗證密碼、APP號碼等資訊，並建立綁定關係。依據註冊狀態回傳對應 JSON 結果。

---

## 輸入參數 (Request Body)

請以 JSON 格式傳送，主要欄位如下：

| 欄位名稱         | 型別      | 必填 | 說明                                   |
|------------------|-----------|------|----------------------------------------|
| LineID           | string    | N    | LINE 識別碼                           |
| InstanceID       | string    | Y    | 裝置識別碼                             |
| PID              | string    | Y    | APP號碼/住戶名稱                       |

### 範例

```json
{
  "LineID": "U161233248c6c02494a1159be2d536c46",
  "InstanceID": "00001-1011111",
  "PID": "1011111"
}
```

---

## 輸出內容 (Response)

### 1. 註冊成功（管理者/首次註冊）

```json
{
  "result": true,
  "KeyID": "加密UID字串"
}
```

### 2. 註冊成功（非管理者，僅建立綁定）

```json
{
  "result": true,
  "binding": {
    "BindingID": 123,
    "UID": 456,
    "LineID": "line_xxx"
  }
}
```

### 3. 註冊失敗（驗證錯誤）

```json
{
  "result": false,
  "errors": [
    { "key": "PassWord", "errors": ["請輸入密碼!!"] },
    { "key": "PID", "errors": ["請輸入APP號碼!!"] }
  ]
}
```

### 4. 設備識別碼錯誤

```json
{
  "result": false,
  "message": "設備識別碼錯誤!!"
}
```

---

## 其他說明
- PID 首次註冊時需與資料庫住戶名稱一致。
- 若設備已註冊，需輸入不同 APP 號碼。
- 欄位驗證錯誤時，errors 為陣列，包含所有錯誤欄位與訊息。

---

## 可能的回應情境

1. 首次註冊：回傳 KeyID。
2. 已註冊僅建立綁定：回傳 binding 物件。
3. 欄位驗證錯誤：回傳 errors 陣列。
4. 設備識別碼錯誤：回傳 message。

---

# InfoCenterController.CheckDefence API 文件

## API 路徑

```
POST /InfoCenter/CheckDefence
```

## 說明

此 API 用於查詢或設定裝置的防禦狀態，並回傳主門狀態、緊急事件等資訊。依據輸入參數與系統狀態，回傳對應的 JSON 結果或空內容。

---

## 輸入參數 (Request Body)

請以 JSON 格式傳送，主要欄位如下：

| 欄位名稱         | 型別      | 必填 | 說明                                   |
|------------------|-----------|------|----------------------------------------|
| InstanceID       | string    | Y    | 裝置識別碼                             |
| DefenceStatus    | int/null  | N    | 設定防禦狀態 (-1: 撤防, 1: 設防) |

### 範例

```json
{
  "InstanceID": "00001-1011111",
  "DefenceStatus": 1
}
```

---

## 輸出內容 (Response)

### 1. 有結果時 (application/json)

```json
{
  "Defence": 1,                // 防禦狀態 (0: 保全未設定, 1: 保全設定)
  "EventCode": 5,              // 緊急事件代碼 (-1: 警報解除, 5: 火災, 14: 地震), 沒有事件則無此欄位
  "MainDoor": 1                // 主門狀態 (0: 大門開啟, 1: 大門關閉)
}
```

#### 欄位說明
- Defence: 目前防禦狀態，若有對應訊息則回傳。
- EventCode: 若有火災、地震、清除等緊急事件，回傳對應代碼。
- MainDoor: 若為本地訊息中心且主門有狀態，回傳主門狀態。

### 2. 無結果時

- 回傳 HTTP 204 No Content 或空內容。

---


# InfoCenterController.SetDefence API 文件

## API 路徑

```
POST /InfoCenter/SetDefence
```

## 說明

此 API 用於設定指定設備的防禦狀態，會將對應設備的 MessageBoard 訊息更新（或建立）為指定的 Defence 狀態。

---

## 輸入參數 (Request Body)

請以 JSON 格式傳送，主要欄位如下：

| 欄位名稱   | 型別      | 必填 | 說明                       |
|------------|-----------|------|----------------------------|
| InstanceID | string    | Y    | 裝置識別碼                 |
| Defence    | int       | Y    | 設定防禦狀態（0: 保全解除, 1: 保全設定） |

### 範例

```json
{
  "InstanceID": "00001-1011111",
  "Defence": 1
}
```

---

## 輸出內容 (Response)

### 1. 設定成功

```json
{ "result": true }
```

### 2. 設定失敗（設備識別碼錯誤）

```json
{ "result": false, "message": "設備識別碼錯誤" }
```

---

# InfoCenter/CheckDefenceStatus API 文件

## API 路徑

```
GET or POST /InfoCenter/CheckDefenceStatus
```

## 說明

此 API 用於查詢指定裝置的防禦狀態，回傳 JSON 格式的 Defence 狀態。

---

## 輸入參數 (Query String 或 Request Body)

| 欄位名稱   | 型別      | 必填 | 說明                       |
|------------|-----------|------|----------------------------|
| InstanceID | string    | Y    | 裝置識別碼                 |

### 範例

```json
{
  "InstanceID": "00001-1011111"
}
```

---

## 輸出內容 (Response)

### 1. 查詢成功

```json
{
  "Defence": 1 // 1: OnGuard, 0: OffGuard
}
```

- Defence: 1 代表設防 (OnGuard)，0 代表未設防 (OffGuard)。

### 2. 查無資料

```json
{
  "Defence": 0
}
```

---

# InfoCenterController.DeleteLineBinding API 文件

## API 路徑

```
POST /InfoCenter/DeleteLineBinding
```

## 說明

此 API 用於刪除指定使用者與 LINE 的綁定關係。

---

## 輸入參數 (Request Body)

請以 JSON 格式傳送，主要欄位如下：

| 欄位名稱   | 型別    | 必填 | 說明                                 |
|------------|--------|------|--------------------------------------|
| InstanceID | string | Y    | 裝置識別碼                           |
| LineID     | string | Y    | LINE 識別碼                          |

### 範例

```json
{
  "InstanceID": "00001-1011111",
  "LineID": "U161233248c6c02494a1159be2d536c46"
}
```

---

## 輸出內容 (Response)

### 1. 刪除成功

```json
{ "result": true }
```

### 2. 刪除失敗（資料錯誤）

```json
{ "result": false, "message": "資料錯誤!!" }
```

---
