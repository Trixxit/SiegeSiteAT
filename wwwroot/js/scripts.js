let currentIndex = 1;
let newsItems = [];

async function fetchNews() {
    try {
        const response = await fetch('/JSON/news.json');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        newsItems = await response.json();
        updateNewsItems();
    } catch (error) {
        console.error('Error fetching news:', error);
    }
}

function updateNewsItems() {
    const leftIndex = (currentIndex - 1 + newsItems.length) % newsItems.length;
    const centerIndex = currentIndex;
    const rightIndex = (currentIndex + 1) % newsItems.length;

    const newsItem1 = document.getElementById('newsItem1');
    const newsItem2 = document.getElementById('newsItem2');
    const newsItem3 = document.getElementById('newsItem3');

    /*
    // Fade out the elements
    newsItem1.classList.add('fade-out');
    newsItem2.classList.add('fade-out');
    newsItem3.classList.add('fade-out');
    */

    // After fade-out duration, update the content and fade-in
    //setTimeout(() => {
        newsItem1.innerHTML = `
            <img src="${newsItems[leftIndex].img}" alt="${newsItems[leftIndex].title}" />
            <h3>${newsItems[leftIndex].title}</h3>
            <p class="placeholder">${newsItems[leftIndex].placeholder}</p>
        `;
        newsItem2.innerHTML = `
            <img src="${newsItems[centerIndex].img}" alt="${newsItems[centerIndex].title}" />
            <h3>${newsItems[centerIndex].title}</h3>
            <p class="placeholder">${newsItems[centerIndex].placeholder}</p>
        `;
        newsItem3.innerHTML = `
            <img src="${newsItems[rightIndex].img}" alt="${newsItems[rightIndex].title}" />
            <h3>${newsItems[rightIndex].title}</h3>
            <p class="placeholder">${newsItems[rightIndex].placeholder}</p>
        `;

        // Remove fade-out and add fade-in
        /*
        newsItem1.classList.remove('fade-out');
        newsItem2.classList.remove('fade-out');
        newsItem3.classList.remove('fade-out');
        newsItem1.classList.add('fade-in');
        newsItem2.classList.add('fade-in');
        newsItem3.classList.add('fade-in');
    }, 1);
            */
}

function scrollNews(direction) {
    console.log("womp");
    const totalItems = newsItems.length;
    currentIndex = (currentIndex + direction + totalItems) % totalItems;
    updateNewsItems();
}

// Ensure scrollNews is in the global scope
window.scrollNews = scrollNews;

// Initial fetch
fetchNews();
