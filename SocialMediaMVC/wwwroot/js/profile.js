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
})