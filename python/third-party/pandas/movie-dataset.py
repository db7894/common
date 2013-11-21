import pandas as pd
import numpy as np

#------------------------------------------------------------
# Initialize the data lens
#------------------------------------------------------------
u_cols = ['user_id', 'age', 'sex', 'occupation', 'zip_code']
users  = pd.read_csv('ml-100k/u.user', sep='|', names=u_cols)

r_cols  = ['user_id', 'movie_id', 'rating', 'timestamp']
ratings = pd.read_csv('ml-100k/u.data', sep='\t', names=r_cols)

m_cols  = ['movie_id', 'title', 'release_date', 'video_release_date', 'imdb_url']
movies  = pd.read_csv('ml-100k/u.item', sep='|', names=m_cols, usecols=range(5))

movie_ratings = pd.merge(movies, ratings)
lens = pd.merge(movie_ratings, users)


#------------------------------------------------------------
# find the top 25 movies as rated by users
#------------------------------------------------------------
#most_rated = lens.title.value_counts()
most_rated = lens.groupby('title').size().order(ascending=False)
print most_rated[:25]

#------------------------------------------------------------
# find the highest rated movies
#------------------------------------------------------------
best_rated = lens.groupby('title').agg({'rating': [np.size, np.mean]})
at_least_100 = best_rated['rating'].size >= 100
print best_rated[at_least_100].sort([('rating', 'mean')], ascending=False)[:25]

#------------------------------------------------------------
# bin our ratins based on user age
#------------------------------------------------------------
most_50 = lens.groupby('movie_id').size().order(ascending=False)[:50]
#plot = users.age.hist(bins=30)
#plot.title("Distribution of user ages")
#plot.ylabel("count of users")
#plot.xlabel("age")

labels = ['0-9', '10-19' ,'20-29', '30-39', '40-49', '50-59', '60-69', '70-79']
lens['age_group'] = pd.cut(lens.age, range(0, 81, 10), right=False, labels=lables)
print lens.groupby('age_group').agg({'rating': [np.size, np.mean]})

lens.set_index('movie_id', inplace=True)
by_age = lens.ix[most_50.ix].groupby(['title', 'age_group'])
print by_age.rating.mean().unstack(1).fillna(0)[10:20]

#------------------------------------------------------------
# difference in ratings by men and woment
#------------------------------------------------------------
lens.reset_index('movie_id', inplace=True)
pivot = lens.pivot_table(rows=['movie_id', 'title'], cols=['sex'], values='rating', fill_value=0)
pivot['diff'] = pivot.M - pivot.F
pivot.reset_index('movie_id', inplace=True)
disagree = pivot[pivot.movie_id.isin(most_50.index)]['diff']
plot = disagree.order().plot(kind='barh', figsize=[9,15])
plot.title('Male vs Female Average Ratings (>0 male)')
plot.ylable('title')
plot.xlable('Average Rating Difference')
