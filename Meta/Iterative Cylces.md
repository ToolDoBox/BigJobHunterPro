Part IV:
Iterative Cycles

Now, we'll start the process of crafting, testing, and refining our project through iterative cycles. Each cycle will involve clarifying what our features need in order to be completed. We will also walk through designing, and implementing these features with a few techniques. Time to put our plans into action!

SPRINT PLANNING:

Welcome to the beginning of the loop. Our goal is to set some very clear, and achievable goals for the user stories that you will be completing within the next cycle[4,5]. You are going to start by looking over the broad user stories that you made earlier, and choose the ones that you believe are the highest priority. If you want to re-evaluate the priority of your user stories, or create new ones, now is the perfect time to do it.
You are going to give yourself a budget of how many story points you think you can accomplish within the next sprint[4,5]. If you have already completed a sprint, then you can use the number of story points you had completed in the previous sprint for your estimate[4,5]. One of the benefits of estimating work this way, is that it is easier to identify a sustainable pace[4]. The goal of the sprint should be consistent forward momentum, not a crazy burst of energy.

1.) Review Backlog: Look over your project's backlog. It's a list of tasks or features waiting to be tackled[4,5].

2.) Define Goals: Clarify what you aim to achieve in this sprint. Link each task to a broader project goal[5].

3.) Select Stories: Pick the user stories or tasks from the backlog you'll focus on this sprint[4,5].

4.) Estimate Effort: Predict how much work each story will need. Consider complexity and potential obstacles. Rough estimate of how long it will take to complete the story, relative to other stories in the project. You can use a system of points, like the fibonacci numbers: 1, 2, 3, 5, 8, 13, 21, 34. Anything higher than an 8 should be broken down[4,5].

5.) Build Sprint Backlog: Create a separate backlog for this sprint. It's your roadmap for the next weeks[4,5].

6.) Set Duration: Decide how long the sprint will be. Typical sprints range from one to four weeks[4,5].

Example Personal Website First Iteration:

Backlog:

- {2 Story Points(SP)} Retire old websites
- {3 SP} About Page
- {3 SP} “Now” Page
- {5 SP} Projects Page (Overview of all projects)
- {3 SP per} Individual Project Pages (~11 from old site, many more not on old site)
- {3 SP} Book(s) Page
- {5 SP} Reflection Page
- {5 SP} Contact Form
- {3 SP} Resume
- {8 SP} Style Guide Page

Start Date: 2023-06-12
End Date: 2023-06-25
Length: 14 days (2 weeks)

Goals:

- {2 SP} Retire old websites
- {3 SP} Create the About Page

I think that these two stories will be plenty of work for the next two weeks. The first cycle is usually the hardest to estimate, often because you are just starting to get into the project's work, and because there are some set-up costs. I’m unsure how difficult it will be to shut down my old domains, and to set up a new one for this site. I also think that the about page should be difficult, because I want to spend the time to get the written content down, and the basic page and text designs down before creating it. I am sure I will have to iterate on these ideas a few times before I feel the about page is finished.
DEFINE COMPLETION CRITERIA:

Now that we have our selected user stories, it's time to get some clarity on what it means to complete them. For each of the user stories you selected for this cycle, list out the 3 to 7 things that need to work in order for you to never have to work on this feature again[5]. If you find that you have more than 7 completion criteria, then it might be a good time to break that story down even further[5].
Completion criteria is traditionally written by the end user, or someone who is close to the end user and has a clear understanding of their perspective of the project[4,5]. Put yourself in your users shoes and think about what you want to see and do with the feature in this user story. The completion criteria should describe clearly what a successful condition is, and be able to be tested by this condition[5]. Completion criteria should also include the work around the main feature, such as testing, documentation, code review, and refactoring. Consider looking at your standards for inspiration to include in your completion criteria.

1.) Main Goal: What does the user want to achieve through this user story[5]?

2.) Breakdown: Deconstruct the User Story into smaller steps. What steps are needed to reach the user story’s main goal[4,5]?

3.) Desired Outcome: What should happen after tasks are done?

4.) Simplify: How can we simplify the mental effort required to reach the desired outcome[4]?

5.) Requirements: How do the requirements that you have created for the project affect the user story? (Functionally, Usability, Aesthetics, Content Quality, and Performance) [1,2]

6.) System Interactions: How does the feature affect other components?

7.) Edge Cases: What unusual scenarios should be accounted for?

8.) Define criteria: What are the things that need to be done in order to consider your user story completed? Write concise criteria for easy team understanding & following. These criteria should be written so that they can be tested and measured[4,5].

Example Completion Criteria Template

⬜ User Story **\_:
As a **Visitor**\_ Story Points: **3**
I want \_to learn about the website owner****\_\_\_\_******
so that I can _understand their background, interests and expertise.********************\_\_********************
Desired Outcome:\_After reading the about page, the\_**\_ visitor should have a clear understanding of my**\_\_**** background, interests, and expertise. They should also_ know how to contact me, or explore more of my work.**
Completion Criteria:
⬜ _The “About” page includes an up-to-date photo_
⬜ \_I can reach the projects page, the “now” page and the contact page from the about page******\_********
⬜ \_The page is enjoyable on a mobile device**\_\_\_\_**
⬜ \_I can read all of the content on the “About” page in under 1 minute, at the average reading speed (238wpm)
⬜ \_The information on the “About” page is written to be timeless, yet should be updated with any “major” events
⬜ \_All content on the “About” page is WCAG and ADA compliant ******************\_\_\_\_******************
⬜ \_The page loads in under 400ms on a slow 3g**\_\_** connection******************\_\_\_******************
⬜ User Story **\_:
As a ******\_******** Story Points: **\_\_\_**
I want **********************\_\_**********************
so that I can ******************\_\_\_\_******************
Desired Outcome:****************\_\_\_\_****************

---

Completion Criteria:
⬜ **********************\_\_\_**********************
⬜ **********************\_\_\_**********************
⬜ **********************\_\_\_**********************
⬜ **********************\_\_\_**********************
⬜ **********************\_\_\_**********************
⬜ **********************\_\_\_**********************
⬜ **********************\_\_\_**********************
⬜ **********************\_\_\_**********************

IDENTIFY COMPONENTS:

Let’s take a moment to consider what our users need in order to complete their tasks through this user story. If the users need to sign in to a website, what buttons or inputs will they need[1,3]? How can we best display the information and content that we have, so that users can understand and use that information[1]?
What elements are users comfortable with seeing for this type of content, what elements do our competitors use to convey this same message[1]? Don’t try to create anything new, just identify the elements that people are used to using to complete these tasks[1,3]. Think about the different icons that could be used, and what they represent[3].
Our goal in this step is to create a list of components and elements that we can use as building blocks for sketching the design of our website. Some common elements include:

- Buttons
- Headings
- Sub-Headings
- Links
- Images
- Text
- Quote Blocks
- Search Boxes
- Checkboxes
- Radio Buttons
- Dropdown Menus
- Text Inputs
- Date/Time Inputs
- Pie Charts/Bar Charts etc.
- File Uploads
  1.) Review completion criteria: What actions and outcomes are required throughout the user story? For instance, a login form might require social media login methods, or a way to access the sign up or forgot password pages.

  2.) Break down complex tasks: Are there sub-components to simplify the design? In our login form example, maybe we can simplify the process by using a one-click login method like signing in with google.

  3.) Identify data components: What data will be stored, retrieved, or displayed? Our login form should collect the email, and password of the user[3].

  4.) Select fitting input types: Which form fields align with collected data? Emails and passwords use different types of inputs. Select the proper type of input, or specify if you will need to create a custom input.

  5.) Consider navigation elements: How will users access and navigate the feature? Can our users access the login or sign out form from anywhere on the page? Can they access the login form from the signup or lost password forms?

  6.) List user interface elements: What inputs, buttons, links or displays are needed? We know that we at least need an email input, and a password input. What other links, buttons or inputs are needed?

  7.) Consistency: How can we ensure that the inputs are consistent with the rest of the site? Is there already a sign up form that we can use as a design example for our login form[7]?

Example Personal Website About Components:

For the about page we don’t have to deal with any input/output handling, so we are only dealing with content elements. Here are a list of elements that could meet all of our completion criteria:

- Up to date image
- Navigation (header nav?)
- Text -> About content paragraph, probably around 250 words max.
- Links to Projects page, now, and contact page.

Example Web App Login Components:

For this example, let's assume we are creating a design for a basic login page for a web app. Here is a list of elements that could be helpful in this situation:

- Labels for the inputs
- Email input
- Password input
- Link to “Forgot Password”
- Link to “Create Account”
- Sign in button
- Login with Account (Google/Github/Facebook etc.)
  SKETCH IDEAS:

Now it's time to get some ideas on paper! The goal is to explore as many ideas as possible. You should only move on from sketching ideas when you have at least 20 different sketches on paper[2]. These sketches should be messy and simple. They don’t have to make sense to anyone but yourself. This is an exercise in getting ideas on paper, not on creating art to hang on your fridge. Use stick figures, squares and circles if you have to. Just get something down on paper.
I recommend starting with 10 2-minute sketches of different ways you can use your components to solve your user stories completion criteria. Then, doing some research and sketching 5 2-minute sketches of how your competitors approached this same problem. And finally, sketching 5 more 2-minute ideas from projects that are completely unrelated to your project. Look at the mona-lisa, architecture, plants, toys, anything that forces you to look away from your project’s traditional views.

1.) Embrace Messiness: Don't worry about perfection, prioritize idea exploration[2,3].

2.) Use Simple Shapes: Leverage squares, rectangles, and circles for UI elements[2].

3.) Include Components: Incorporate the elements from your components list[3].

4.) Consider Navigation: Include how users will access and move through the feature.

5.) Account For Different Screen Sizes: Plan for responsiveness in your sketches[2,3].

6.) Embrace Constraints: Set limitations to inspire unique solutions in your sketches.

7.) Use References: Collect images and resources to inspire your Project design sketches.

8.) Mashup Concepts: Combine unrelated elements or ideas to create inventive sketches.

9.) Set A 2-Minute Timer: Keep sketches quick and focused on generating ideas[2].

10.) Sketch Multiple Ideas: Generate a variety of concepts for the feature. Try to create at least 15 different sketches for this feature[2].

11.) Review And Refine: Iterate on sketches to improve and narrow down ideas.

Example Personal Website About Sketches:

This was one of the sketches that was inspired by another person's about page. I like that the headers make it easy to skim information. The image has enough whitespace to not feel overwhelming. The “About Me” lets me know exactly where I am on the site. I like that the top nav menu saves space, but I personally don’t like collapsable menus.

This was one that I had sketched early on. The large image space will be great for other project pages. The top nav is simple, and effective. The design itself is very common, but it is common for a reason: It works. Forces you to scroll, which isn’t a bad thing. Definitely a strong contender.

This design was one of my last ones, I think it was number 19 or so. I actually was looking at some minimalist line art, and really liked the simplicity and boldness of a single black line. I also looked around my room and realized that I use this design of white background with thin, bold black for everything.
Outside of that, I really grew to love the look of the top nav, it's simple, unique and tells you where you are.
The image has a black border around it, with plenty of whitespace to contrast with it. The image needs to fit the rest of the style however. The text is short and sweet, so it's more likely to be read, rather than skimmed. The links at the bottom are closer to your fingers on mobile, and offer you a simple choice for exploring the site. I’ll probably end up with this for my final design.CREATE WIREFRAMES:

A wireframe is a blueprint for a screen, which serves as the rough mockup of the layout of the components and content on a page[1,2]. The wireframe is designed to be a reference for the structure, and priority of elements of a page[1,2]. As such, it should avoid any of the lower level details of design, such as color, images, or typography[2]. Instead, limit yourself to: one font(arial, or something generic), white, black, a few shades of gray, and blue text to denote hyperlinks[2]. The key tool you will be playing with in your wireframe is contrast, and layout between the various elements and components, which should guide your eye through your page[1,2].
While your wireframe should be a rough blueprint, it should use the real text content you have gathered or created previously[2]. Using real content, as opposed to lorem ipsum, will give you a clear idea of the size of text blocks, and keep your work tailor to your needs.
While you are creating your wireframe, you should annotate it with notes about how the final product will react or function[2]. Make notes of where links will lead to, and how you envision the elements acting[1].Write your notes such that, if you were to wipe your memory of this project, you could take this wireframe and implement its design.
While it may be tempting to create the magnum opus of wireframes, the value of creating a detailed wireframe diminishes over time. Once you have a chance to layout your components, and get a clear view of your structure and priority of elements, then your wireframe has been valuable. Anymore time spent adding details to a wireframe, is time that could be spent adding details to the final product.
Once you have finished the first version of your wireframe, ask yourself a few questions: Is this design consistent with other pages on the site[1]? Do your eyes gravitate towards the most important element on the screen[1,2,3]? Is there clear feedback when a user interacts with an element[1,2,3]? What happens if a user entered information incorrectly[1,2,3]? What happens when a user submits information correctly, but then the system breaks down[2,3]? Can you figure out exactly where you are on the website, and can you figure out where to go from here[1,2]?

1.) Pick Your Favorite Sketch: Out of the ideas that you have sketched, pick your favorite one.

2.) Consider Organization: How is the information organized and displayed? One of the key aspects that wireframes should address is the layout. You should reference your information architecture at this point[1,2].

3.) Draw Boxes: Represent components with boxes. Include every element from your chosen sketch. The goal of the wireframe is to get a sense of layout and visual hierarchy, which can be accomplished with simple boxes[2].

4.) Functionality: How will this work and how do users interact with it?

5.) Use Real Content: Include accurate labels, text, and layout specs[2].

6.) Indicate Images: Use empty boxes with [IMAGE] labels for placeholders[2].

7.) Visual Hierarchy: Where does the user’s eye first go when they look at the page[1,2,7]?

8.) Simplify: Look for ways to simplify the steps required to complete the action[7].

9.) Consistency: Ensure that the design is consistent with the website’s personality and design[7].

10.) Keep Wireframe Simple: Use Arial font, shades of gray, and blue for text hyperlinks. AVOID using color for your wireframes, instead use shades of gray and focus on contrast[2].

11.) Contrast: How can you use contrast in visual design to draw the user's attention to essential aspects of the interface[2,3,7]?

12.) Screen Sizes: Design for various devices and choose a responsive grid[2,3].

13.) Touch Interactions: Design interactions around mobile touch zones[2].

Example Personal Website About Wireframe:

Once I brought my sketch into my wireframe software I realized how much contrast could be an issue when dealing with just black and white. The blue shade I use is mostly just to denote where I am thinking of placing links, however I may add a shade of blue in the final design. The laptop design was also a challenge, because of the excess of whitespace, which makes it feel empty, rather than purposeful. More work is needed, but it's fine for now.

BUILD FRONTEND:

Now it's time to take all of the decisions and designs you’ve made so far, and turn them into a living and breathing website. One of the most challenging steps when starting from a black screen is figuring out how to approach the implementation. Even with the wireframe, user story and completion criteria already created, it can be overwhelming.
One technique that I recommend is to start by figuring out what the biggest concerns are for creating your design. Then break those big concerns down into smaller concerns. Break down these smaller concerns until you either: A) Know what you need to do B) Have a guess on what you could do or C) can put your concern into words that can be easily found through google.
Then start experimenting until you’ve conquered the little concerns. Don’t try to approach the big components or layouts of your page all at once, start by figuring out how to break it down into the smallest chunk that you can figure out. Once you finish that chunk, you move onto the next small concern. If you ever run into a concern where you don’t know what to do, have a guess or can google it, then you know you need to break it down even further.

1.) Break Down Components: Break down the wireframe into each component, and the layout of the feature. Start by thinking about how you are going to approach the layout and HTML structure of the component.

2.) HTML Structure: Start by adding the content directly onto the page. Use proper tags and elements to improve accessibility and SEO. In our login form example, this would mean bringing any labels, and inputs into the HTML[2,3].

3.) Add Content: Insert text, images, and other elements needed for this feature[2,3].

4.) CSS Styles: Use your wireframe as a guide to style the form. CSS will help you recreate colors, sizes, and positioning[2,3].

5.) Implement UI Patterns: Build consistent and familiar interface elements for a better UX[2,3].

6.) User Interaction: Define how users will interact with the form. JavaScript can handle this. For example, what happens when they click the 'submit' button[2,3]?

7.) Feedback: Add responses for user actions. Let users know when they've entered incorrect data or successfully logged in[1,2,3].

8.) Utilize Sound: What unique sounds can establish a sense of personality? Don’t overdo it.

9.) Modularize code: Organize your frontend code into reusable components and modules[3].

10.) Follow best practices: Ensure clean, maintainable code by adhering to coding standards and guidelines[3].

Example Personal Website About Frontend:

I started by deciding that I was going to create the initial designs for a mobile browser. I made this decision based on my understanding that the majority of the users that are going to see this website are going to be viewing on a mobile device, based on the research of my core set of users.
I think the most complex and challenging part of this design is the header, so I wanted to break it down to the point I could start working on it. I repeated this process until I finished the header, then I moved onto the image, then the paragraph.
At this point I felt a little
indifferent about the design I had chosen. While it was the best of my sketches, it was different than I was expecting. I hit this point where you build the first version and it causes me to pause and get stuck on where to go or fix from here.
I think that the font needs to be changed, and the paragraph needs some trimming, as well as a custom style for in-text links. I need to revisit using contrast in the design, because all of the text and links have the same priority, which means nothing has priority. While the black on white fits my personal theme, it makes it too serious, and doesn’t really portray my personality the way I envisioned. I will make these changes in the future.
ENSURE RESPONSIVENESS:

One of the things that easily gets overlooked when creating a web project is making sure the page looks well on most devices. Now that you have the front end for one screen size created, it's time to make it dynamic enough to account for the majority of screens.One way of creating responsive websites is to design for specific sizes, such as the top 20% of screen resolutions for browsing the web[9]:

- 1920x1080 (~8.8%)
- 360x800 (~7.4%)
- 1366x768 (~6%)

While this approach may cover the most popular sizes, we can cover a much wider range of devices by choosing screen width ranges to test and design our pages[3]. These ranges cover the majority of devices, however may have situations where an element breaks at a specific size[3]:

- Mobile Devices: 300px -> 500px (Breakpoint @ 600px)
- Tablets (vertical): 600px -> 900px (Breakpoint @ 900px)
- Tablets (horizontal): 900px -> 1100px (Breakpoint @ 1200px)
- Desktop/Laptop: 1200px -> (Who knows where the limits of the desktop end…)

Another approach is to start at one point, such as the smallest phone resolution, and then scale up the size until design breaks, and add a breakpoint to address the issue[3]. This could also be applied by working from the largest resolution, and working down towards a mobile resolution. However, While this approach will cover all scenarios between these points, it is more time intensive and requires more breakpoints.
It’s important to note that just because these are the most popular screen resolutions, doesn’t mean that this is the resolution of your page within the browser for these screen sizes. It can be difficult to know exactly how much space the search bar and bookmark bar take up. For example, on my phone the screen size is 1179x2556 resolution, while the rendered screen size is closer to 393x852. But, the page only really sees about 393x660 of that total screen size when the search bar is visible. Sometimes I put my browser in 75% scale in order to see better, which puts my page resolution at 524x880. All of this to say, it can be tricky to cover every case. Right now mobile devices make up nearly 60% of the global market share, while desktop browsing is at 38%[9].

1.) Fluid Layout: Adopt a fluid grid layout for a flexible, adaptive design[3].

2.) Image Scaling: Set a max-width for images. This ensures they resize correctly on various devices.

3.) Media Breakpoints: Utilize media queries. They allow CSS customization based on device width.

4.) Mobile First: Begin with mobile design. Enhance progressively for larger screens.

5.) Test Different Screens: Check responsiveness across multiple device sizes[3].

6.) Optimized Navigation: Adapt menus and elements for different devices.

7.) Minimize Scrolling: Condense content for smaller screens.

8.) Relative Fonts: Use responsive typography. Consider relative font sizes for improved readability[3].

9.) Touch-Friendly Elements: Design easy-to-access, touch-friendly buttons[3].

10.) Adaptable Orientation: Accommodate both portrait and landscape layouts.

11.) Prioritize Content: On small screens, streamline content to essential info[3].

12.) Diverse Inputs: Account for touch, keyboard, and stylus inputs in your design.

13.) Responsive Media: Ensure images and videos scale responsively across devices[3].

14.) Flexible Elements: Avoid fixed-width elements to prevent layout issues[3].

Example Personal Website About Responsiveness:

I ended up deciding to use the golden ratio for the spacing on the left and right side of both the tablet view, and the laptop view. I found that half of the ratio worked really well for centering it. It looks pretty consistent between the different screen sizes now.
ENSURE ACCESSIBILITY:

Now that we have a website that you can view on nearly any device, it's time to make sure that nearly anyone can view and use our website. If you are thinking about skipping this step to save time, or you don’t think this impacts you personally, consider the Curb-Cut effect. The Curb-Cut effect describes the benefits to the majority of people that come from designing for those with disadvantages, or those who come from an underrepresented group[10]. If you’ve ever had to wheel a shopping cart in or out of a storefront that had a curb, you’ve benefited from accessibility design.
This same effect also plays a role when using or creating digital experiences as well. Designing with accessibility in mind can create a better user experience, help improve your website's SEO, and help your project reach a wider audience. One benefit I have found from designing with accessibility in mind, is the ability to move quickly through websites with keyboard only controls. Once you get the hang of it, keyboard shortcuts are often much faster than using a mouse.
I recommend checking out the ARIA(Accessible Rich Internet Applications) documentation[11] and the WCAG(Web Content Accessibility Guidelines) documentation[12] for more information on how to include accessibility into your designs.

1.) Contrast Colors: Text should be clearly visible against the background[3,7].

2.) Alt Text: Add descriptions to images for screen readers[3,7].

3.) Semantic HTML: Use correct tags for headings, lists, and others[3].

4.) Clear Links: Ensure navigation with informative link text[3].

5.) Keyboard Navigation: All interactive elements should be keyboard-accessible[3].

6.) Follow Guidelines: Use appropriate ARIA roles and comply with WCAG.

7.) Text Alternatives: Provide transcripts for audio, captions for videos.

8.) Logical Organization: Elements should be orderly and hierarchical.

9.) Accessible Forms: Label fields clearly and offer helpful error prompts[3].

10.) Consistent Design: Maintain similar layouts throughout for easy navigation[3,7].

11.) Resizable Text: Text should be resizable without compromising readability or functionality[3].

12.) Validate Code: Confirm error-free HTML, CSS, and JavaScript to avoid accessibility issues.
Example Personal Website About Accessibility:

I ran my code through an html checker, as well as an accessibility checker. One of the things I didn’t realize I was missing was an ‘alt’ attribute on the image. The ‘alt’ attribute is used to display text if the image can’t load, and its often used in screen readers. I also found out that I was missing out on a high contrast outline, which is important for users navigating through the site without a mouse.

DESIGN DATABASE MODELS:
Now is the time to use your sketches for your high level data model and turn them into a high fidelity database. Develop a clear and accurate database schema to improve data access, processing, and storage[8].

1.) Update Sketch: Collect the following from your data model sketches, and update them if needed:
a.) Identify Entities: Spot the critical elements and their relationships in your database.

b.) Define Attributes: Detail properties of each entity.

c.) Set Relationships: Define how entities connect and interact[8].

2.) Consider Indexing: Boost query performance by indexing searched attributes often[8].

3.) Enforce Constraints: Apply rules and validation to ensure data consistency.

4.) Plan Growth: Keep scalability and future expansion in mind while designing the database. Don’t pre-maturely scale.

5.) Review & Refine: Check your model for efficiency, consistency, and best practices.

BUILD BACKEND:
Create the engine that powers your website, handles your logic, processes data, and ensures smooth functionality. A solid backend is crucial for a stable, scalable, and secure website[8].

1.) Break Down Design: Think about the initial wireframe design and the new front end design. Identify the points, and times where you will need to make a call to the backend. For example, for our login form, we will need to make a call to the api when the “submit” button is pressed.

2.) Plan API Structure: Once you have identified the endpoints, HTTP methods, and data structures needed, start planning out the structure of how the back end will handle these different calls[8].

2.) Implement Data Models: Translate database sketches into actual data structures. Add the constraints and ensure that the models[8].

3.) Implement Authentication: Safeguard user data by incorporating secure authentication methods[8].

4.) Ensure Data Validation: Implement validation checks to maintain data integrity and security[8].

5.) Design Error Handling: Create a robust system by handling errors gracefully and consistently[8].

6.) Optimize Performance: Use caching, indexing, and other techniques to boost server-side efficiency. Only do this when you find a bottleneck, avoid unnecessary and premature optimization[8].

7.) Establish API Versioning: Manage changes to the API with a clear versioning system[8].

8.) Test Backend Functionality: Ensure reliability by thoroughly testing API endpoints and logic[8].

9.) Utilize API Documentation Tools: Keep API documentation up to date and accessible for easy reference[8].

HANDLE ERRORS/FORGIVENESS:

Your project might be user friendly when it works, but is it still friendly when something goes wrong? Taking advantage of smart error handling techniques goes a long way into creating a user friendly project[1,2,7]. The most important aspect of creating an error message is making sure that the message is clear, meaningful, and tell the user what to do or where to go next[1,2].
One way to help prevent errors from occurring is by providing consistent, clear feedback on user interactions[2,7]. If you are noticing users making a lot of errors at a specific part of your project, it may be a sign that there is too much going on, or that it is too confusing and causing mistakes[7]. Another way to help prevent errors is to make them nearly impossible to occur[1,7]. We can accomplish this by putting stops in place that prevent users from taking actions that will cause errors[1,7].
One thing that makes a big difference to users is offering forgiveness on certain actions[7]. For example, if a user deletes an item, then can enter ‘ctrl-z’ or press ‘undo’ in order to revert their change. Another way to help prevent mistakes or errors is to ask for confirmation when making a potentially damaging decision[7]. When deleting an account, a large database, or something of value, make sure to ask for confirmation[7]. Be careful to not turn these confirmation messages into spam or they will be ignored, and more frustrating for the user than they will be beneficial to them[7].

1.) Anticipate User Mistakes: Identify potential user errors and prepare solutions. For our login form example, consider what happens if the user enters in their password incorrectly, or their email is not in a valid format[7].

2.) Validate User Input: Check data for correctness before processing it further. Don’t forget to trim spaces, deal with capitalization problems, or limit number sizes before processing inputs[3,6,7,8].

3.) Implement Error Messages: Show clear, informative messages for user errors. Good error messages are polite, clear and informative: "Oops! The email you entered doesn't seem valid. Please check it and try again." Bad error messages are vague and unhelpful: "Invalid entry." [3,6,7,8]

4.) Use Confirmation Dialogs: Prompt users to confirm critical actions like deletion. Don’t overuse these dialogs or else they will become ignored and distracting[7].

5.) Provide Undo Options: Allow users to revert accidental changes when possible[7].

6.) Implement Error Logging: Track and record errors for debugging and analysis[8].

7.) Handle Backend Errors: Catch and handle server-side errors gracefully. Hopefully your server won’t shut down at the sight of a weird input[8].

8.) Error Recovery: Ensure users can resume their tasks after encountering errors. When should the system attempt to fix user errors automatically and when should it avoid doing so[8]?

9.) Create Fallbacks: Offer alternatives when a feature or resource is unavailable.

10.) Test For Errors: Simulate user mistakes and test error handling mechanisms.

QUALITY ASSURANCE CHECKLIST:
Before the feature is considered completed, please review each of these aspects of your feature. This is a checklist of things to consider to ensure effective design and functionality of your feature.

⬜ Check visual hierarchy: Ensure clear contrast and proper organization of elements.
⬜ Confirm design consistency: Match website personality and overall branding.
⬜ Verify navigation clarity: Users should know where they are at all times.
⬜ Test cross-browser compatibility: Ensure functionality across various browsers.
⬜ Test on multiple devices: Verify functionality across various screen sizes/devices.
⬜ Assess mobile responsiveness: Verify optimal performance on mobile devices.
⬜ Optimize for SEO: Implement best practices for search engine optimization.
⬜ Run Lighthouse tests: Check performance, accessibility, SEO, and best practices.
⬜ Perform automated testing: Use tools to test frontend and backend functionality.
⬜ Test accessibility features: Ensure compliance with accessibility guidelines.
⬜ Review content quality: Check grammar, spelling, and readability of text.
⬜ Inspect server performance: Ensure backend can handle anticipated traffic.
⬜ Validate database integrity: Check for proper data storage and retrieval.
⬜ Examine error handling: Verify appropriate responses to user and system errors.
⬜ Test edge cases: Simulate unusual user actions and system conditions.
⬜ Check security measures: Ensure protection against common vulnerabilities.
⬜ Review user flow: Verify smooth transitions and interactions for users.
⬜ Test API endpoints: Confirm expected behavior and error handling of APIs.
⬜ Examine load times: Optimize for fast rendering and asset delivery.
⬜ Validate form functionality: Test input validation, error messages, and submission.
⬜ Monitor server logs: Check for errors, warnings, and performance issues.
⬜ Assess scalability: Confirm the system can handle growth in traffic and data.
⬜ Examine URL structure: Check for clean, descriptive, and SEO-friendly URLs.
⬜ Validate image optimization: Ensure proper sizing, compression, and formats.
⬜ Review CSS and JavaScript: Check for efficient, well-organized code.
⬜ Inspect 404 and error pages: Verify informative and user-friendly error handling.
⬜ Check for broken links: Ensure all internal and external links function properly.
⬜ Test multimedia elements: Verify video, audio, and interactive components function.
⬜ Confirm feature completion: Have all aspects of the user story been addressed.

SPRINT REVIEW:

Once you’ve hit your end date for this sprint it’s time to check in and see how you're doing, even if you haven’t finished all your tasks yet[4]. Start by looking over your completed work, and calculate how many story points worth of work are finished, to determine how much you should tackle in the next sprint[4,5]. You should also look over your backlog of user stories, features and tasks and prioritize them again, based on what you’ve learned in this sprint[5].
Depending on your project type, and the work that you’ve completed on your last sprint, you may be able to publish what you’ve completed[4,5]. Getting these smaller chunks of your project done can be a motivating way to release the project over time, rather than all at once[5]. If you have any supporters or users that you can show your finished work to, now would be a great time to get feedback.
Now that the sprint is over, take some time to reflect on a few things: What went well during that sprint[5]? What are some of the challenges that you ran into[5]? What are 3 things that you would like to change for the next sprint[5]?

1.) Review Sprint Goals: Review if goals were met, partially met, or not met[5].

2.) Demo Completed Work: Showcase the feature or task to stakeholders for feedback[4,5].

3.) Gather Feedback: Collect input from team members and stakeholders on improvements[4,5].

4.) Discuss Challenges: Address any obstacles or issues faced during the sprint[5].

5.) Reflect On Successes: Identify the aspects of the sprint that went well[5].

6.) Analyze Performance: Determine if time and resources were used efficiently[5].

7.) Identify Learnings: Discuss new insights or knowledge gained during the sprint[5].

8.) Look For Ways To Improve: Develop a plan to address areas of weakness or gaps[5].

9.) Adjust Future Sprints: Incorporate feedback and learnings into upcoming sprints[5].

10.) Celebrate Achievements: Acknowledge the team's hard work and accomplishments[5].

Example Personal Website Sprint Review:

At the beginning of this sprint I set the goal to complete these user stories:

- {2 SP} Retire old websites
- {3 SP} Create the About Page

14 days, 5 Story Points Completed.
So the next sprint I’ll plan for about 5-6 story points worth and see how that feels.

As I was retiring my old websites I found one that had been up for the past 5 years. I went through the whole website to save and archive it, and found my first blog article.

“It’s me(The author of this webpage), I always seen blogging and creating a website as a cool thing I could do when I was older and had something interesting to write about, but then I realized that if I had waited for something interesting to write about, then I would never have started a blog.”(Christian from 2018)

There was something a little poetic about finding this now. I felt like I was fulfilling some prophecy I had set for myself years prior. (I also wrote in long, run-on sentences apparently…)

I think that everything went pretty smoothly. I was actually ahead of my end date by a few days. I think that I could have, and probably do need to, revisit the design and wireframes a little bit more. I could have easily spent much more time on that part of the process, especially since I felt it could have been better.
The most challenging part of this sprint was creating the responsive design for the about page. I realized that because I didn’t create wireframes or sketches for the tablet sizes, I didn’t know how I wanted it to look. I ended up spending more time messing with the visual coding, than it would have taken me to do a few sketches beforehand.
The three things I am going to do going forward are:

1. Spend a little more time sketching and designing before moving forward. 2) Spend more time thinking about the desktop and tablet views. Mostly because I had intended and designed the site for mobile views. 3) Set up more styling that I can use for all of the rest of the web pages.

From this point on, I guess the only thing there is to do is…

Repeat Until Complete.
