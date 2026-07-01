export const required = (value, name) => {
  if (value === null || value === undefined || String(value).trim() === "") return `${name} is required.`;
  return "";
};

export const email = (value) => {
  if (!value) return "Email is required.";
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) ? "" : "Invalid email address.";
};

export const positive = (value, name) => (Number(value) > 0 ? "" : `${name} is required.`);

export function validateUser(form) {
  return [
    required(form.fullName, "Full name"),
    required(form.username, "Username"),
    email(form.email),
    required(form.password, "Password"),
    form.role === "Admin" || form.role === "Teacher" ? "" : "Role must be Admin or Teacher."
  ].filter(Boolean);
}

export function validateClass(form) {
  return [required(form.className, "Class name")].filter(Boolean);
}

export function validateSubject(form) {
  return [required(form.subjectName, "Subject name"), positive(form.teacherUserId, "Teacher")].filter(Boolean);
}

export function validateStudent(form) {
  return [
    required(form.fullName, "Full name"),
    form.gender === "Male" || form.gender === "Female" ? "" : "Gender must be Male or Female.",
    required(form.phone, "Phone"),
    email(form.email),
    positive(form.classId, "Class"),
    form.status === "Active" || form.status === "Inactive" ? "" : "Status must be Active or Inactive."
  ].filter(Boolean);
}

export function validateAttendance(form) {
  return [
    positive(form.studentId, "Student"),
    positive(form.classId, "Class"),
    positive(form.subjectId, "Subject"),
    positive(form.markedByUserId, "Marked by user"),
    form.status === "Present" || form.status === "Absent" || form.status === "Late"
      ? ""
      : "Status must be Present, Absent, or Late."
  ].filter(Boolean);
}
