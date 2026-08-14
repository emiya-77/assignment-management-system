"use client";

import {
  FormEvent,
  useEffect,
  useState,
} from "react";

import {
  ClipboardList,
  Eye,
  Send,
} from "lucide-react";

import { api } from "@/lib/api";

import {
  Assignment,
} from "@/types/assignment";

import {
  CreateSubmissionRequest,
  Submission,
  SubmissionStatus,
  UpdateSubmissionRequest,
} from "@/types/submission";

import { Button } from "@/components/ui/button";

import {
  Card,
  CardContent,
} from "@/components/ui/card";

import {
  Badge,
} from "@/components/ui/badge";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

import {
  Textarea,
} from "@/components/ui/textarea";

import {
  Label,
} from "@/components/ui/label";


export default function StudentDashboard() {
  const [assignments, setAssignments] =
    useState<Assignment[]>([]);

  const [submissions, setSubmissions] =
    useState<Submission[]>([]);

  const [isEditingSubmission, setIsEditingSubmission] =
    useState(false);

  const [selectedAssignment, setSelectedAssignment] =
    useState<Assignment | null>(null);

  const [answer, setAnswer] =
    useState("");

  const [isLoading, setIsLoading] =
    useState(true);

  const [isSubmitting, setIsSubmitting] =
    useState(false);

  const [error, setError] =
    useState("");


  useEffect(() => {
    loadAssignments();
  }, []);


  async function loadAssignments() {
    try {
      setError("");
      setIsLoading(true);

      const [
        assignmentsResponse,
        submissionsResponse,
      ] = await Promise.all([
        api<Assignment[]>("/Assignments"),
        api<Submission[]>("/Submissions"),
      ]);

      setAssignments(assignmentsResponse);

      setSubmissions(submissionsResponse);

    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to load assignments."
      );
    } finally {
      setIsLoading(false);
    }
  }

  function getSubmission(
    assignmentId: number
  ) {
    return submissions.find(
      (submission) =>
        submission.assignmentId === assignmentId
    );
  }


function openAssignment(
  assignment: Assignment
) {
  const submission =
    getSubmission(assignment.id);

  setSelectedAssignment(assignment);

  setAnswer(
    submission?.answer ?? ""
  );

  setIsEditingSubmission(false);

  setError("");
}


  async function handleSubmit(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    if (!selectedAssignment) {
      return;
    }

    try {
      setError("");
      setIsSubmitting(true);

      const request: CreateSubmissionRequest = {
        answer: answer.trim(),
      };

      await api<Submission>(
        `/Submissions/assignment/${selectedAssignment.id}`,
        {
          method: "POST",
          body: JSON.stringify(request),
        }
      );

      setSelectedAssignment(null);
      setAnswer("");

      await loadAssignments();

    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to submit assignment."
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleUpdateSubmission(
  event: FormEvent<HTMLFormElement>,
  submission: Submission
) {
  event.preventDefault();

  try {
    setError("");
    setIsSubmitting(true);

    const request: UpdateSubmissionRequest = {
      answer: answer.trim(),
    };

    await api<Submission>(
      `/Submissions/${submission.id}`,
      {
        method: "PUT",
        body: JSON.stringify(request),
      }
    );

    setIsEditingSubmission(false);

    await loadAssignments();

  } catch (error) {
    setError(
      error instanceof Error
        ? error.message
        : "Failed to update submission."
    );
  } finally {
    setIsSubmitting(false);
  }
}


  return (
    <div className="space-y-6">

      <div>

        <h1 className="text-3xl font-bold tracking-tight">
          My Assignments
        </h1>

        <p className="mt-1 text-muted-foreground">
          View and submit assignments for your enrolled courses.
        </p>

      </div>


      {error && (

        <div className="rounded-lg border border-destructive/50 bg-destructive/10 p-4 text-sm text-destructive">
          {error}
        </div>

      )}


      {isLoading ? (

        <div className="flex min-h-64 items-center justify-center text-sm text-muted-foreground">
          Loading assignments...
        </div>

      ) : assignments.length === 0 ? (

        <Card>

          <CardContent className="flex min-h-64 flex-col items-center justify-center text-center">

            <ClipboardList className="mb-4 size-10 text-muted-foreground" />

            <h2 className="font-semibold">
              No assignments found
            </h2>

            <p className="mt-1 text-sm text-muted-foreground">
              You currently do not have any assignments.
            </p>

          </CardContent>

        </Card>

      ) : (

        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">

          {assignments.map((assignment) => {
          
            const submission =
            getSubmission(assignment.id);

          return (
          
          <Card
              key={assignment.id}
              className="flex flex-col"
            >

              <CardContent className="flex flex-1 flex-col p-5">

                <div className="flex items-start justify-between gap-4">

                  <div>

                    <p className="text-sm text-muted-foreground">
                      {assignment.code}
                    </p>

                    <h2 className="mt-1 font-semibold">
                      {assignment.title}
                    </h2>

                  </div>

                  <div className="flex flex-col items-end gap-2">

                    <Badge variant="secondary">
                      {assignment.maximumMarks} Marks
                    </Badge>

                    {submission ? (

                      <Badge
                        variant={
                          submission.status === SubmissionStatus.Graded
                            ? "default"
                            : "secondary"
                        }
                      >
                        {getSubmissionStatusLabel(
                          submission.status
                        )}
                      </Badge>

                    ) : (

                      <Badge variant="outline">
                        Not Submitted
                      </Badge>

                    )}

                  </div>

                </div>


                <p className="mt-4 line-clamp-3 text-sm text-muted-foreground">
                  {assignment.description}
                </p>


                <div className="mt-5 space-y-2 text-sm">

                  <div>

                    <span className="text-muted-foreground">
                      Course:
                    </span>{" "}

                    <span className="font-medium">
                      {assignment.courseCode}
                    </span>

                  </div>


                  <div>

                    <span className="text-muted-foreground">
                      Subject:
                    </span>{" "}

                    <span className="font-medium">
                      {assignment.subjectName}
                    </span>

                  </div>


                  <div>

                    <span className="text-muted-foreground">
                      Teacher:
                    </span>{" "}

                    <span className="font-medium">
                      {assignment.teacherName}
                    </span>

                  </div>


                  <div>

                    <span className="text-muted-foreground">
                      Deadline:
                    </span>{" "}

                    <span className="font-medium">
                      {new Date(
                        assignment.deadline
                      ).toLocaleString()}
                    </span>

                  </div>

                </div>


                <Button
                  className="mt-6 w-full"
                  onClick={() =>
                    openAssignment(assignment)
                  }
                >
                  <Eye />

                  {submission
                    ? "View Submission"
                    : "View Assignment"}
                </Button>

              </CardContent>

            </Card>

          )})}

        </div>

      )}


      {/* Assignment Dialog */}

      <Dialog
        open={!!selectedAssignment}
        onOpenChange={(open) => {
          if (!open) {
            setSelectedAssignment(null);
            setAnswer("");
          }
        }}
      >

        <DialogContent className="max-h-[90vh] overflow-y-auto">

          <DialogHeader>

            <DialogTitle>
              {selectedAssignment?.title}
            </DialogTitle>

            <DialogDescription>
              {selectedAssignment?.code}
            </DialogDescription>

          </DialogHeader>


          {selectedAssignment && (
          (() => {
              const submission =
                getSubmission(selectedAssignment.id);

              return (
            <div className="space-y-6">

              <div className="space-y-2 text-sm">

                <div>

                  <span className="text-muted-foreground">
                    Course:
                  </span>{" "}

                  <span className="font-medium">
                    {selectedAssignment.courseCode}
                    {" - "}
                    {selectedAssignment.courseName}
                  </span>

                </div>


                <div>

                  <span className="text-muted-foreground">
                    Subject:
                  </span>{" "}

                  <span className="font-medium">
                    {selectedAssignment.subjectCode}
                    {" - "}
                    {selectedAssignment.subjectName}
                  </span>

                </div>


                <div>

                  <span className="text-muted-foreground">
                    Teacher:
                  </span>{" "}

                  <span className="font-medium">
                    {selectedAssignment.teacherName}
                  </span>

                </div>


                <div>

                  <span className="text-muted-foreground">
                    Deadline:
                  </span>{" "}

                  <span className="font-medium">
                    {new Date(
                      selectedAssignment.deadline
                    ).toLocaleString()}
                  </span>

                </div>


                <div>

                  <span className="text-muted-foreground">
                    Maximum Marks:
                  </span>{" "}

                  <span className="font-medium">
                    {selectedAssignment.maximumMarks}
                  </span>

                </div>

              </div>


              <div>

                <h3 className="font-semibold">
                  Assignment Description
                </h3>

                <p className="mt-2 whitespace-pre-wrap text-sm text-muted-foreground">
                  {selectedAssignment.description}
                </p>

              </div>


              {submission ? (

                isEditingSubmission ? (

                  <form
                    onSubmit={(event) =>
                      handleUpdateSubmission(
                        event,
                        submission
                      )
                    }
                    className="space-y-4"
                  >

                    <div className="space-y-2">

                      <Label htmlFor="edit-answer">
                        Your Answer
                      </Label>

                      <Textarea
                        id="edit-answer"
                        value={answer}
                        onChange={(event) =>
                          setAnswer(
                            event.target.value
                          )
                        }
                        className="min-h-40"
                        maxLength={10000}
                        required
                      />

                    </div>


                    <div className="flex gap-3">

                      <Button
                        type="button"
                        variant="outline"
                        className="flex-1"
                        disabled={isSubmitting}
                        onClick={() => {
                          setAnswer(submission.answer);
                          setIsEditingSubmission(false);
                        }}
                      >
                        Cancel
                      </Button>


                      <Button
                        type="submit"
                        className="flex-1"
                        disabled={
                          isSubmitting ||
                          !answer.trim()
                        }
                      >
                        {isSubmitting
                          ? "Saving..."
                          : "Save Changes"}
                      </Button>

                    </div>

                  </form>

                ) : (

                  <div className="space-y-5 rounded-lg border p-4">

                    <div className="flex items-center justify-between gap-4">

                      <h3 className="font-semibold">
                        Your Submission
                      </h3>

                      <Badge>
                        {getSubmissionStatusLabel(
                          submission.status
                        )}
                      </Badge>

                    </div>


                    <div className="space-y-2">

                      <p className="text-sm font-medium">
                        Your Answer
                      </p>

                      <p className="whitespace-pre-wrap text-sm text-muted-foreground">
                        {submission.answer}
                      </p>

                    </div>


                    <div className="grid gap-4 text-sm sm:grid-cols-2">

                      <div>

                        <p className="text-muted-foreground">
                          Submitted
                        </p>

                        <p className="font-medium">
                          {new Date(
                            submission.submittedAt
                          ).toLocaleString()}
                        </p>

                      </div>


                      {submission.updatedAt && (

                        <div>

                          <p className="text-muted-foreground">
                            Last Updated
                          </p>

                          <p className="font-medium">
                            {new Date(
                              submission.updatedAt
                            ).toLocaleString()}
                          </p>

                        </div>

                      )}

                    </div>


                    {submission.status ===
                      SubmissionStatus.Graded && (

                      <div className="space-y-4 rounded-lg border p-4">

                        <div>

                          <p className="text-sm text-muted-foreground">
                            Marks
                          </p>

                          <p className="text-2xl font-bold">
                            {submission.marks}
                            {" / "}
                            {selectedAssignment.maximumMarks}
                          </p>

                        </div>


                        {submission.feedback && (

                          <div>

                            <p className="text-sm font-medium">
                              Teacher Feedback
                            </p>

                            <p className="mt-1 whitespace-pre-wrap text-sm text-muted-foreground">
                              {submission.feedback}
                            </p>

                          </div>

                        )}

                      </div>

                    )}


                    {selectedAssignment.allowSubmissionUpdate &&
                      new Date(
                        selectedAssignment.deadline
                      ) > new Date() && (

                        <Button
                          className="w-full"
                          variant="outline"
                          onClick={() => {
                            setAnswer(submission.answer);
                            setIsEditingSubmission(true);
                          }}
                        >
                          Edit Submission
                        </Button>

                      )}

                  </div>

                )

              ) : (

                <form
                  onSubmit={handleSubmit}
                  className="space-y-4"
                >

                  <div className="space-y-2">

                    <Label htmlFor="answer">
                      Your Answer
                    </Label>

                    <Textarea
                      id="answer"
                      value={answer}
                      onChange={(event) =>
                        setAnswer(event.target.value)
                      }
                      placeholder="Write your answer here..."
                      className="min-h-40"
                      maxLength={10000}
                      required
                    />

                  </div>


                  <Button
                    type="submit"
                    className="w-full"
                    disabled={
                      isSubmitting ||
                      !answer.trim()
                    }
                  >
                    <Send />

                    {isSubmitting
                      ? "Submitting..."
                      : "Submit Assignment"}
                  </Button>

                </form>

              )}
            </div>);
            })()
          )}

        </DialogContent>

      </Dialog>

    </div>
  );
}

function getSubmissionStatusLabel(
  status: SubmissionStatus
) {
  switch (status) {
    case SubmissionStatus.Submitted:
      return "Submitted";

    case SubmissionStatus.UnderReview:
      return "Under Review";

    case SubmissionStatus.Graded:
      return "Graded";

    case SubmissionStatus.Returned:
      return "Returned";

    default:
      return "Unknown";
  }
}