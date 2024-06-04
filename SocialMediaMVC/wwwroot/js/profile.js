window.addEventListener('load', () => {
    const countPosts = () => document.querySelectorAll('[data-post-id]').length
    if (countPosts() >= 5) {
        const postsContainer = document.querySelector('#posts-container')
        const postTemplate = document.querySelector('#post-template')
        const loadMorePosts = (skip) => {
            return fetch(`/posts?skip=${skip}&take=5`, {
                method: 'GET'
            }).then(response => response.json()).catch(err => {
                console.error(err)
                return []
            })
        }

        const renderPosts = (posts) => {
            posts.forEach(post => renderPost(postsContainer, postTemplate, post))
        }

        const observer = new IntersectionObserver(entries => {
            entries.forEach(async entry => {
                if (entry.isIntersecting) {
                    const posts = await loadMorePosts(countPosts())
                    if (posts.length > 0) {
                        renderPosts(posts)
                    }
                    if (posts.length % 5 !== 0) {
                        observer.disconnect()
                    }
                }
            })
        })

        observer.observe(document.querySelector('#end-of-posts'))
    }

    const followBtn = document.querySelector('button[data-following]')
    const totalFollowers = document.querySelector('#total-followers')
    followBtn.addEventListener('click', async () => {
        followBtn.disabled = true
        const following = followBtn.getAttribute('data-following') === 'true'
        const userId = followBtn.getAttribute('data-user-id')
        const response = await fetch(`/users/${userId}/followers`, {
            method: following ? 'DELETE' : 'POST',
        })

        if (response.ok) {
            followBtn.setAttribute('data-following', !following)
            followBtn.textContent = following ? 'Follow' : 'Unfollow'
            followBtn.disabled = false
            totalFollowers.textContent = `${parseInt(totalFollowers.textContent) + (following ? -1 : 1)} followers`
        }
    })
})