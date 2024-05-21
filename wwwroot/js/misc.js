window.checkInternet = function () {
    return navigator.onLine;
};

window.logMessage = function (message) {
    console.log(message);
};

window.fetchWithProgress = async function (url, dotNetObjRef) {
    console.log("fetchWithProgress called with URL:", url);
    const response = await fetch(url);
    const reader = response.body.getReader();
    const contentLength = +response.headers.get('Content-Length');

    let receivedLength = 0;
    const chunks = [];

    while (true) {
        const { done, value } = await reader.read();
        if (done) break;

        chunks.push(value);
        receivedLength += value.length;

        console.log("Progress:", receivedLength, contentLength); // Log progress
        await dotNetObjRef.invokeMethodAsync('ReportProgress', receivedLength, contentLength);
    }

    const chunksAll = new Uint8Array(receivedLength);
    let position = 0;
    for (let chunk of chunks) {
        chunksAll.set(chunk, position);
        position += chunk.length;
    }

    const result = new TextDecoder("utf-8").decode(chunksAll);
    console.log("Fetch complete"); 

    return result;
};