
window.addEventListener('load', () => {
    const posts = document.querySelectorAll('[data-post-id]')

    posts.forEach(post => {
        let abortController
        post.querySelector('.like-button').addEventListener('click', function () {
            if (abortController) {
                abortController.abort()
            }
            abortController = new AbortController()
            handlePostLike(this, post, abortController.signal)
        })
    })
})
function likePost(postId, signal) {
    return fetch(`/posts/likes/${postId}`, {
        method: 'POST',
        signal
    })
}

function unlikePost(postId, signal) {
    return fetch(`/posts/likes/${postId}`, {
        method: 'DELETE',
        signal
    })
}
function handlePostLike(button, post, signal) {
    const postId = post.getAttribute('data-post-id')
    const likesCount = post.querySelector('.likes-count')
    if (button.classList.contains('liked')) {
        likesCount.textContent = parseInt(likesCount.textContent) - 1
        button.classList.remove('liked')

        unlikePost(postId, signal).catch(() => {
            if (!signal.aborted) {
                likesCount.textContent = parseInt(likesCount.textContent) + 1
                button.classList.add('liked')
            }
        }).finally(() => {
            abortController = null
        })
    } else {
        likesCount.textContent = parseInt(likesCount.textContent) + 1
        button.classList.add('liked')

        likePost(postId, signal).catch(() => {
            if (!signal.aborted) {
                likesCount.textContent = parseInt(likesCount.textContent) - 1
                button.classList.remove('liked')
            }
        }).finally(() => {
            abortController = null
        })
    }
}

function renderPost(container, template, post) {
    const postElement = template.content.firstElementChild.cloneNode(true)
    postElement.dataset.postId = post.id
    postElement.querySelector('.post-author').textContent = post.author.userName
    postElement.querySelector('.post-content').textContent = post.content
    postElement.querySelector('.likes-count').textContent = post.totalLikes
    postElement.querySelector('.like-button').classList.toggle('liked', post.likedByClient)

    const carousel = postElement.querySelector('[data-post-carousel="0"]')
    if (post.images.length > 0) {
        carousel.dataset.postCarousel = post.id
        const templateItem = carousel.querySelector('.carousel-item')
        const clone = templateItem.cloneNode(true)
        Array.from(templateItem.parentElement.children).forEach(child => child.remove())
        post.images.forEach((image, index) => {
            const imageElement = clone.cloneNode(true)
            if (index > 0) {
                imageElement.classList.remove('active')
            }
            imageElement.querySelector('img').src = image
            imageElement.querySelector('img').alt = `Image ${index + 1}`
            carousel.querySelector('.carousel-inner').appendChild(imageElement)
        })
        if (post.images.length > 1) {
            carousel.querySelector('.carousel-control-prev').dataset.bsTarget = `[data-post-carousel="${post.id}"]`
            carousel.querySelector('.carousel-control-next').dataset.bsTarget = `[data-post-carousel="${post.id}"]`
            const indicators = carousel.querySelector('.carousel-indicators')
            const indicatorClone = indicators.querySelector('button').cloneNode(true)
            indicators.querySelectorAll('button').forEach(indicator => indicator.remove())

            post.images.forEach((_, index) => {
                const indicator = indicatorClone.cloneNode(true)
                if (index === 0) {
                    indicator.classList.add('active')
                }
                indicator.dataset.bsTarget = `[data-post-carousel="${post.id}"]`
                indicator.dataset.bsSlideTo = index
                indicator.setAttribute('aria-label', `Image ${index + 1}`)
                indicators.appendChild(indicator)
            })
        } else {
            carousel.querySelector('.carousel-indicators').remove()
            carousel.querySelector('.carousel-control-prev').remove()
            carousel.querySelector('.carousel-control-next').remove()
        }
    } else {
        carousel.remove()
    }

    if (post.author.profileImage) {
        postElement.querySelector('.profile-photo-placeholder').remove()
        const img = document.createElement('img')
        img.src = post.author.profileImage
        img.alt = `${post.author.userName}'s photo`
        img.classList.add('profile-photo')
        img.setAttribute(document.querySelector('.profile-photo').getAttributeNames()[0], "")
        postElement.querySelector('.card-header').prepend(img)
    }

    container.appendChild(postElement)
    // Handle like button click
    const likeButton = postElement.querySelector('.like-button')
    let abortController
    likeButton.addEventListener('click', function () {
        if (abortController) {
            abortController.abort()
        }
        abortController = new AbortController()

        handlePostLike(this, postElement, null)
    })

}