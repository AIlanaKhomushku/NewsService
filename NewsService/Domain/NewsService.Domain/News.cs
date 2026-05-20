using NewsService.Domain.NewsService.Domain.Base;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.NewsService.Domain.Exceptions;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.ValueObjects;

namespace NewsService.Domain.NewsService.Domain
{
    ///
    public class News : Entity<Guid>
    {

        public Title Title { get; private set; }
        public Content Content { get; private set; }
        public DateTime CreationData { get; }
        public DateTime? ModificationData { get; private set; } = null;//дата изменения в новости
        public NewsStatus NewsStatus { get; private set; } = NewsStatus.Created;
    
        public Author Author { get; } = default!;
        private readonly ICollection<Reaction> _reactions = [];
        public IReadOnlyCollection<Reaction> Reactions => _reactions.ToList().AsReadOnly();



        private readonly ICollection<Comment> _comments = [];

        public IReadOnlyCollection<Comment> Comments => _comments.ToList().AsReadOnly();

        protected News() { }
        public News(
            Title title,
            Content content,
            Author author,
            DateTime creationData
            )
            : this(Guid.NewGuid(), author, content, creationData, title) { }

        protected News(Guid id,
            Author author,
            Content content,
            DateTime creationData,
            Title title,
            DateTime? modificationData = null)
            : base(id)
        {
            Author = author ?? throw new ArgumentNullValueException(nameof(author));

            Content = content ?? throw new ArgumentNullValueException(nameof(content));
            Title = title ?? throw new ArgumentNullValueException(nameof(title));

            if (modificationData is not null && modificationData < creationData)
                throw new InvalidModificationDataException(this, modificationData.Value);

            CreationData = creationData;
            ModificationData = modificationData;
        }
        /// <summary>
        /// меняем название
        /// </summary>
        /// <param name="newtitle"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullValueException"></exception>

        public bool SetTitle(Title newtitle)
        {
            if (newtitle == null) throw new ArgumentNullValueException(nameof(newtitle));
            if (Title == newtitle)
                return false;
            Title = newtitle;
            return true;
        }
        /// <summary>
        /// меняем контент
        /// </summary>
        /// <param name="newContent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullValueException"></exception>
        public bool SetContent(Content newContent)
        {
            if (newContent == null) throw new ArgumentNullValueException(nameof(newContent));
            if (Content == newContent)
                return false;
            Content = newContent;
            return true;
        }
        /// <summary>
        /// время измения
        /// </summary>
        /// <param name="modificationData"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullValueException"></exception>
        /// <exception cref="InvalidModificationDataException"></exception>

        public bool SetModificationData(DateTime modificationData)
        {
            if (CreationData > modificationData) throw new InvalidModificationDataException(this, modificationData);
            if (ModificationData > modificationData) throw new InvalidModificationDataException(this, modificationData);
            if (ModificationData == modificationData)
                return false;
            ModificationData = modificationData;
            return true;
        }

        /// <summary>
        /// меняем статус
        /// </summary>
        /// <param name="newStatus"></param>
        /// <returns
       
        public bool SetStatus(Author author, NewsStatus newStatus)
        {
            if(author is null) throw new ArgumentNullValueException(nameof(author));
            if (NewsStatus == newStatus) return false;
            NewsStatus = newStatus;
            return true;
        }
        /// <summary>
        /// Поставить реакцию
        /// </summary>
        /// <param name="newReaction"></param>
        /// <returns></returns>
        public bool SetReaction(User user, NewsReaction newReaction)
        {
            if (user == null) throw new ArgumentNullValueException(nameof(user));
            if (NewsStatus != NewsStatus.Published) throw new InvalidNewsStatusForUserActionException("reaction", NewsStatus);

            var existing = _reactions.FirstOrDefault(r => r.User.Id == user.Id);
            if (existing != null)
            {
                if (existing.Type == newReaction) return false;
                existing.UpdateType(newReaction);
                return true;
            }

            var reaction = new Reaction(this, user, newReaction);
            _reactions.Add(reaction);
            return true;
        }
        /// <summary>
        /// переопределяем ToString
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {
            //var reactionText = _userreactions.Count == 0
            //    ? "no reaction"
            //    : string.Join("; ", _userreactions.Select(r => $"key={r.Key}, value={r.Value}"));

            var commentText = _comments.Count == 0 ? "no comment " : string.Join("; ", _comments.Select(c => $"{c.User.ToString()}: {c.Content.ToString()}"));
            //return $"{Title.ToString()} {Content.ToString()} ({CreationData} {NewsStatus}) {reactionText} - {commentText}";
            var reactionsSummary = _reactions.Count == 0 ? "" : string.Join(",", _reactions.GroupBy(r => r.Type).Select(g => $"{g.Key}:{g.Count()}"));
            return $"{Title.ToString()} {Content.ToString()} {reactionsSummary} ({CreationData} {NewsStatus}) - {commentText}";
        }
        /// <summary>
        /// добавляем комментарий
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullValueException"></exception>
        public bool SetComment(Comment comment)
        {
            if (comment is null) throw new ArgumentNullValueException(nameof(comment));
            if (NewsStatus != NewsStatus.Published) throw new InvalidNewsStatusForUserActionException("comment", NewsStatus);

            _comments.Add(comment);
            return true;
        }
    }
}