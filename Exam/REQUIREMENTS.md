```
I have an exam task to build a Dance School Management System in ASP.NET Core Razor Pages.

EXACT Requirements:
1. Dance Styles: 12 styles (ballet, contemporary, jazz, hip-hop, salsa, bachata, tango, ballroom, pole fitness, aerial silks, breakdancing, K-pop)

2. Studios: 4 studios with different sizes
    - Studio A: 120m² (standard floor, mirrors)
    - Studio B: 80m² (standard floor, mirrors)
    - Studio C: 60m² (poles installed, exclusively for pole fitness)
    - Studio D: 100m² (high ceiling with aerial rigging for aerial silks)

3. Class Levels: beginner, intermediate, advanced, professional

4. Schedule:
    - Monday-Saturday: 18:00-21:00
    - Sunday: 10:00-14:00

5. Pricing Structure:
    - Drop-in class: €15 (10-class card €120 valid 3 months)
    - Monthly unlimited single-style: €75
    - Monthly unlimited all-styles: €130
    - When 10-class card has 2 classes remaining: Alert student
    - Expired cards: Block booking

6. Trial Classes:
    - Students can bring friends for FREE trial class
    - Only once per person per style
    - Example: Maria tried free salsa trial, but can still do free trial in hip-hop
    - Track trial usage (who tried what style already)

7. Seasonal Showcases:
    - Track who's eligible (attendance above 80%, level intermediate+)
    - Performance opportunities

8. Student Management:
    - Track purchased packages (10-class cards, monthly unlimited, all-styles, single-style)
    - Usage against purchased packages
    - Alert when package expiring
    - Track trial usage per style
    - Some students trying expired cards (system must block this automatically)

TASK:
1. Analyze these requirements carefully
2. Design complete data model with all entities and relationships:
    - DanceStyle (12 styles)
    - Studio (4 studios with sizes)
    - Class (schedule, level, style, studio, price)
    - Student (registration, profile)
    - Package (10-class card, monthly unlimited)
    - Booking (student books class, uses package)
    - TrialUsage (track who tried which style)
    - Attendance (track for 80% threshold)
3. Plan all Razor Pages needed:
    - Student registration and profile
    - Package purchase and tracking
    - Class schedule (filtered by style, level, studio, time)
    - Booking system (check package validity, trial eligibility)
    - Trial friend invitation system
    - Attendance tracking
    - Showcase eligibility tracking
    - Studio management
    - Reports (popular times, package usage, trial conversions)
4. Business logic requirements:
    - Package validation (expiry, remaining classes)
    - Trial eligibility check (one per style per person)
    - Alert system (2 classes remaining, package expiring)
    - Attendance calculation (80% threshold)
    - Showcase eligibility
    - Studio capacity management

```
