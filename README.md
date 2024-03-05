# Blackbird.io Google Analytics

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Google Analytics is a web analytics service provided by Google that allows website owners to track and analyze various aspects of their website's performance and user interaction. It provides valuable insights into how users engage with a website, where they come from, and what actions they take while on the site. Google Analytics offers a wide range of features to help website owners understand their audience, optimize their content, and improve their online presence.

## Before setting up

Before you can connect you need to make sure that:

- You have a Google Analytics account and access to the Google Analytics property you want to retrieve data from.
- You have the Property ID of the digital asset that you want to track and analyze using Google Analytics. To find your property ID, go to Google Analytics website, admin page, select the desired GA4 property from the list. From the middle column, click Property Settings and on the top right corner you will see your Property ID.

## Connecting

1. Navigate to apps and search for Google Analytics. If you cannot find Google Analytics then click _Add App_ in the top right corner, select Google Analytics and add the app to your Blackbird environment.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Google Analytics connection'.
4. Fill in the Property ID of the digital asset you want to get data about.
5. Click _Sign in with Google_.
6. As a new window pops up, follow the instructions that Google gives you.
7. When you return to Blackbird, confirm that the connection has appeared and the status is _Connected_.

![GoogleAnalyticsConnection](image/README/GoogleAnalyticsConnection.png)

## Actions

- **Get page data** Get metrics of a specific page from the last x days
	- New users
	- Total users
	- Active users
	- Conversion rate
	- Transactions
	- Sessions
	- Scrolled users
	- Conversions
	- Bounce rate
	- Summary

## Example

The example below shows a bird that updpates a Zendesk article after being translated by machine translation. Then the bird is delayed for two weeks, to gather enough data. And based on Google Analytics' metrics on that same Zendesk article, in this case whether there were more than 1000 total users visiting said article for the last 14 days, a new MemoQ project is created and the translated article is sent for human revision. 

![AnalyticsExample](image/README/AnalyticsExample.png)

Below are some of the available Google Analytics metrics to be chosen in order to make data-driven decisions.

![DataPointsAnalytics](image/README/DataPointsAnalytics.png)

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
