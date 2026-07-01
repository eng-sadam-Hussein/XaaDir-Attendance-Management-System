import { Award, Database, Globe2, ShieldCheck } from "lucide-react";
import PageHeader from "../components/PageHeader";

export default function About() {
  const cards = [
    { icon: ShieldCheck, title: "Role-Based Access", text: "Admin can view all data. Teachers can only view assigned subjects and own reports." },
    { icon: Database, title: "Database Connected", text: "React connects to ASP.NET Core Web API, and the API connects to SQL Server XaaDirDB." },
    { icon: Globe2, title: "Modern React UI", text: "Built with React JS, Tailwind CSS, reusable components, routes, validation, and toasts." },
    { icon: Award, title: "Guideline Ready", text: "Includes CRUD, reports, search, authentication, Context API, and API integration." }
  ];

  return (
    <>
      <PageHeader title="About XaaDir App" subtitle="Smart Attendance Made Simple using React JS, Tailwind CSS, ASP.NET Core Web API, ADO.NET, and SQL Server." />
      <div className="grid gap-5 md:grid-cols-2 xl:grid-cols-4">
        {cards.map((card) => {
          const Icon = card.icon;
          return (
            <div className="card" key={card.title}>
              <Icon className="text-xaadirBlue" size={34} />
              <h3 className="mt-4 text-xl font-black">{card.title}</h3>
              <p className="muted mt-2">{card.text}</p>
            </div>
          );
        })}
      </div>
      <div className="card mt-7 bg-gradient-to-br from-blue-50 to-orange-50">
        <h2 className="text-2xl font-black">School Schedule</h2>
        <p className="muted mt-2">School days: Saturday to Wednesday. Time: 7:00 AM to 12:30 PM.</p>
      </div>
    </>
  );
}
