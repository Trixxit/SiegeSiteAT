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
