window.checkInternet = function () {
    return navigator.onLine;
};

window.logMessage = function (message) {
    console.log(message);
};

window.openNew = function (url) {
    window.open(url, '_blank');
};

window.fetchData = async function (url) {
    console.log(`Fetching and converting ${url}...`);
    const response = await fetch(url);
    const text = await response.text();
    console.log(`Fetched and converted ${url}!`);
    return text;
};



window.fetchWithCorsAnywhere = async function (url) {
    const corsProxy = "https://cors-anywhere.herokuapp.com/";
    const fullUrl = corsProxy + url;

    const response = await fetch(fullUrl);
    if (!response.ok) {
        throw new Error('Network response was not ok ' + response.statusText);
    }
    const text = await response.text();
    return text;
};

window.fileInterop = {
    readFile: async function (filePath) {
        const response = await fetch(filePath);
        if (response.ok) {
            return await response.text();
        }
        return null;
    },
    writeFile: async function (filePath, content) {
        const blob = new Blob([content], { type: "application/json" });
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = filePath;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
};